﻿using Microsoft.AspNetCore.Mvc;
using sportWorld.Data;
using sportWorld.Models;

namespace sportWorld.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> CategoryList = _db.Categories.ToList();
            return View(CategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost] // Specify actions when the form is submitted 
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString()) // Custom validation
            {
                ModelState.AddModelError("name", "Category Name can not match Display Order");
            }
            if (ModelState.IsValid) // Validate before adding to db
            {
				_db.Categories.Add(category); // The EF Core will keep track of change to db
				_db.SaveChanges(); // Changes will only be executed on db when calling SaveChanges()
				return RedirectToAction("Index");
			}
            return View();
        }
		public IActionResult Edit(int? id)
		{
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _db.Categories.Find(id);
            // Other method to get category from db
            // Category? categoryFromDb2 = _db.Categories.FirstOrDefault(u => u.Id == id);
            // Category? categoryFromDb3 = _db.Categories.Where(u=>u.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }
			return View(categoryFromDb);
		}
		[HttpPost] // Specify actions when the form is submitted 
		public IActionResult Edit(Category category)
		{
			if (ModelState.IsValid) // Validate before adding to db
			{
				_db.Categories.Update(category); // The EF Core will keep track of change to db
				_db.SaveChanges(); // Changes will only be executed on db when calling SaveChanges()
				return RedirectToAction("Index");
			}
			return View();

		}
	}
}
