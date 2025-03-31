using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Models.ViewModels;
using sportWorld.Utility;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace sportWorld.Areas.Customer.Controllers
{
	[Area("Customer")] // Associate the controller with the area
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly int _pageSize;
		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_pageSize = 2;
		}

		public IActionResult Index()
		{
			IQueryable<Product> productList = _unitOfWork.Product.GetAllQueryable(includeProperties: "Category");

			HomeVM homeVM = new HomeVM()
			{
				pageNumber = 1,
				TotalPages = (int)Math.Ceiling(productList.Count() / (double)_pageSize),
				productList = CreatePage(productList, 1, _pageSize),
				categoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				})
			};
			
			return View(homeVM);
		}
		[HttpPost]
		public IActionResult Index(HomeVM homeVM, string action)
		{
			IQueryable<Product> productList = _unitOfWork.Product.GetAllQueryable(includeProperties: "Category");

			// If the pagination buttons are clicked
			if (action == "next")
			{
				homeVM.pageNumber += 1;
			}
			else if (action == "previous")
			{
				homeVM.pageNumber -= 1;
			}
			// When the paginations buttons are NOT clicked
			else
			{
				if (homeVM.categoryFilter == "all")
				{
					// Reset page number when returning to view all categories
					homeVM.pageNumber = 1;
				}
				// Filter by product category first
				if (!String.IsNullOrEmpty(homeVM.categoryFilter) && homeVM.categoryFilter != "all")
				{
					productList = productList.Where(u => u.CategoryId.ToString() == homeVM.categoryFilter);
					// Reset page number for the first time
					homeVM.pageNumber = 1;
				}
				// Filter by product name second
				if (!String.IsNullOrEmpty(homeVM.nameFilter))
				{
					productList = productList.Where(u => u.Name.ToLower().Contains(homeVM.nameFilter.ToLower()));
					// Reset page number for the first time
					homeVM.pageNumber = 1;
				}
			}

			// Filter by product category first
			if (!String.IsNullOrEmpty(homeVM.categoryFilter) && homeVM.categoryFilter != "all")
			{
				productList = productList.Where(u => u.CategoryId.ToString() == homeVM.categoryFilter);
			}
			// Filter by product name second
			if (!String.IsNullOrEmpty(homeVM.nameFilter))
			{
				productList = productList.Where(u => u.Name.ToLower().Contains(homeVM.nameFilter.ToLower()));
			}

			homeVM.TotalPages = (int)Math.Ceiling(productList.Count() / (double)_pageSize);
			homeVM.productList = CreatePage(productList, homeVM.pageNumber, _pageSize);
			homeVM.categoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString()
			});

			return View(homeVM);
		}
		public IEnumerable<Product> CreatePage(IQueryable<Product> source, int pageIndex, int pageSize)
		{
			return source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
		}
		public IActionResult Details(int productId)
		{
			ShoppingCart cart = new()
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
