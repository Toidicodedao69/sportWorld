﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Models.ViewModels;

namespace sportWorld.Areas.Admin.Controllers
{
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> ProductList = _unitOfWork.Product.GetAll().ToList();

			return View(ProductList);
		}
		public IActionResult Upsert(int? id)
		{
			//ViewBag.CategoryList = CategoryList; // Transfer data to View and only last during current http request
			ProductVM productVM = new()
			{
				CategoryList = _unitOfWork.Category     // Projection in EF Core to dynamically convert each Category object to SelectListItem
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Product = new Product()
			};

			if (id == 0 || id == null)
			{
				// Create
				return View(productVM);
			}
			else
			{
				// Update
				productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
				return View(productVM);
			}
		}
		[HttpPost] // Specify actions when the form is submitted 
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
			if (ModelState.IsValid) // Validate before adding to db
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;

				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");

					using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}

				_unitOfWork.Product.Add(productVM.Product); // The EF Core will keep track of change to db
				_unitOfWork.Save(); // Changes will only be executed on db when calling SaveChanges()
				TempData["success"] = "Product created successfully!"; // Show notification for only 1 render
				return RedirectToAction("Index");
			}
			else
			{
				productVM = new()
				{
					CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
					{
						Text = u.Name,
						Value = u.Id.ToString()
					})
				};

				return View(productVM);
			}
		}
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

			if (productFromDb == null)
			{
				return NotFound();
			}
			return View(productFromDb);
		}
		[HttpPost] // Specify actions when the form is submitted 
		public IActionResult Edit(Product product)
		{
			if (ModelState.IsValid) // Validate before adding to db
			{
				_unitOfWork.Product.Update(product); // The EF Core will keep track of change to db
				_unitOfWork.Save(); // Changes will only be executed on db when calling SaveChanges()
				TempData["success"] = "Product updated successfully!"; // Show notification for only 1 render
				return RedirectToAction("Index");
			}
			return View();

		}
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

			if (productFromDb == null)
			{
				return NotFound();
			}
			return View(productFromDb);
		}
		[HttpPost, ActionName("Delete")] // Specify actions when the form is submitted 
		public IActionResult DeletePOST(int? id) // This action method's name is still "Delete" as specified in ActionName annotation
		{
			Product? obj = _unitOfWork.Product.Get(u => u.Id == id);

			if (obj == null)
			{
				return NotFound();
			}

			_unitOfWork.Product.Remove(obj);
			_unitOfWork.Save();
			TempData["success"] = "Product deleted successfully!"; // Show notification for only 1 render
			return RedirectToAction("Index");
		}
	}
}
