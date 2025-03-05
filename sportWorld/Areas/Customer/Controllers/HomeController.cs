using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace sportWorld.Areas.Customer.Controllers
{
    [Area("Customer")] // Associate the controller with the area
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
			IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }
		public IActionResult Details(int productId)
		{
            ShoppingCart cart = new ()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
		    };

			return View(cart);
		}
        [HttpPost]
        [Authorize] // Must log in to add products
		public IActionResult Details(ShoppingCart cart)
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            cart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == cart.ProductId);

            if (cartFromDb != null)
            {
                // Update cart
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
				_unitOfWork.Save();
			}
			else
            {
				// Add new cart
				_unitOfWork.ShoppingCart.Add(cart);
				_unitOfWork.Save();
                // Set number of unique products in shopping cart
				HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
			}

            TempData["success"] = "Cart updated successfully";

			return RedirectToAction(nameof(Index));     // nameof find all Action pages in the current Controller
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
