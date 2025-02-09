using Microsoft.AspNetCore.Mvc;
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
				_db.Categories.Add(category);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
            return View();

        }
    }
}
