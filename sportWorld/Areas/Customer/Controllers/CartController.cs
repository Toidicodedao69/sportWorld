using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Models.ViewModels;
using sportWorld.Utility;
using System.Net.Quic;
using System.Security.Claims;

namespace sportWorld.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		[BindProperty] // Keep properties for all action methods after populating -> Only needs to populate once
		public ShoppingCartVM cartVM { get; set; }
		
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
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

			cartVM.CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product").ToList();

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

			return RedirectToAction(nameof(OrderConfirmation), new { OrderHeaderId = cartVM.OrderHeader.Id });
		}
		public IActionResult OrderConfirmation(int OrderHeaderId)
		{
			return View(OrderHeaderId);
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
