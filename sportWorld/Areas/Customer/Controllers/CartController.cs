using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Models.ViewModels;
using sportWorld.Utility;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Net.Quic;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace sportWorld.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailSender _emailSender;
		[BindProperty] // Keep properties for all action methods after populating -> Only needs to populate once
		public ShoppingCartVM cartVM { get; set; }
		
		public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
		{
			_unitOfWork = unitOfWork;
			_emailSender = emailSender;
		}
		public IActionResult Index()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			cartVM = new()
			{
				CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product").ToList(),
				OrderHeader = new()
			};

			foreach(var cart in cartVM.CartList)
			{
				cart.Product.Category = _unitOfWork.Category.Get(u => u.Id == cart.Product.CategoryId);
				cart.Price = GetPriceBasedOnQuantity(cart);
				cartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}

            return View(cartVM);
        }
		public IActionResult OrderSummary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM cartVM = new()
			{
				CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product").ToList(),
				OrderHeader = new()
			};

			cartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			cartVM.OrderHeader.Name = cartVM.OrderHeader.ApplicationUser.Name;
			cartVM.OrderHeader.PhoneNumber = cartVM.OrderHeader.ApplicationUser.PhoneNumber;
			cartVM.OrderHeader.StreetAddress = cartVM.OrderHeader.ApplicationUser.StreetAddress;
			cartVM.OrderHeader.PostalCode = cartVM.OrderHeader.ApplicationUser.PostalCode;
			cartVM.OrderHeader.City = cartVM.OrderHeader.ApplicationUser.City;
			cartVM.OrderHeader.State = cartVM.OrderHeader.ApplicationUser.State;


			foreach (var cart in cartVM.CartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				cartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}

			return View(cartVM);
		}
		[HttpPost]
		[ActionName("OrderSummary")]
		public IActionResult OrderSummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			cartVM.CartList = _unitOfWork.ShoppingCart
				.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product").ToList();

			cartVM.OrderHeader.ApplicationUserId = userId;
			cartVM.OrderHeader.OrderDate = System.DateTime.Now;

			// DO NOT populate cartVM.OrderHeader.applicationUser
			// Because EF Core will duplicate the record of applicationUser when adding OrderHeader to db
			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
			
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				// Customer user -> pending payment
				cartVM.OrderHeader.OrderStatus = SD.StatusPending;
				cartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			}
			else
			{
				// Company user -> delayed payment
				cartVM.OrderHeader.OrderStatus = SD.StatusApproved;
				cartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
			}

			foreach (var cart in cartVM.CartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				cartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}

			_unitOfWork.OrderHeader.Add(cartVM.OrderHeader); 
			_unitOfWork.Save(); // Add OrderHeader before adding OrderDetail

			foreach(var cart in cartVM.CartList)
			{
				OrderDetail orderDetail = new()
				{
					OrderHeaderId = cartVM.OrderHeader.Id, // Can access orderHeaderId because it was added when calling _unitOfWork.Save()
					ProductId = cart.ProductId,
					Count = cart.Count,
					Price = cart.Price
				};

				_unitOfWork.OrderDetail.Add(orderDetail);
			}

			_unitOfWork.Save();

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				var domain = Request.Scheme + "://" + Request.Host.Value;
				// Customer user -> redirect to Stripe payment
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain + $"/Customer/Cart/OrderConfirmation?id={cartVM.OrderHeader.Id}",
					CancelUrl = domain + "/Customer/Cart/Index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

				foreach(var cart in cartVM.CartList)
				{
					var lineItemOptions = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(cart.Price * 100),
							Currency = "aud",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = cart.Product.Name
							}
						},
						Quantity = cart.Count
					};
					options.LineItems.Add(lineItemOptions);
				}

				var service = new SessionService();
				Session session = service.Create(options);

				// PaymentIntenId = null now, it will be populated once payment sucess
				_unitOfWork.OrderHeader.UpdateStripePaymentId(cartVM.OrderHeader.Id, session.Id, session.PaymentIntentId); 
				_unitOfWork.Save();

				// Redirect to Stripe
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = cartVM.OrderHeader.Id });
		}
		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties:"ApplicationUser");

			if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
			{
				// Customer user
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusApproved, SD.StatusApproved);
					_unitOfWork.Save();
				}

				var domain = Request.Scheme + "://" + Request.Host.Value;
				var order_status_url = domain + $"/Admin/Order/Details?orderId={orderHeader.Id}";

				_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "Sport World - Order Confirmation", 
				$"<p>Your order has been placed successfully<br>" +
				$"<strong>Order number:</strong> {orderHeader.Id}<br>" +
				$"You can check the order status <a href={order_status_url}>here</a><br><br>" +
				$"Thank you for shopping at Sport World!</p>");
			}

			// Clear shopping cart items count
			HttpContext.Session.Clear();

			// Empty ShoppingCart
			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
				.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();

			TempData["success"] = "Order placed successfully!";

			return View(id);
		}
		
		public IActionResult Plus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

			cart.Count += 1;
			_unitOfWork.ShoppingCart.Update(cart);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}
		public IActionResult Minus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

			if (cart.Count == 1)
			{
				_unitOfWork.ShoppingCart.Remove(cart);
				HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
				.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count() - 1);
			}
			else
			{
				cart.Count -= 1;
				_unitOfWork.ShoppingCart.Update(cart);
			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}
		public IActionResult Delete(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

			_unitOfWork.ShoppingCart.Remove(cart);
			// Update session before db save changes
			HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
				.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count() - 1);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		private double GetPriceBasedOnQuantity(ShoppingCart cart)
		{
			if (cart.Count <= 20)
			{
				return cart.Product.Price;
			}
			else
			{
				return cart.Product.Price20;
			}
		}
    }
}
