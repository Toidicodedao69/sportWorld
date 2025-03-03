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
