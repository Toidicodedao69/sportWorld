using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Models.ViewModels;
using System.Net.Quic;
using System.Security.Claims;

namespace sportWorld.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM cartVM = new()
			{
				CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product").ToList(),
				OrderTotal = 0
			};

			foreach(var cart in cartVM.CartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				cartVM.OrderTotal += cart.Price * cart.Count;
			}

            return View(cartVM);
        }
		public IActionResult OrderSummary()
		{
			return View();
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
