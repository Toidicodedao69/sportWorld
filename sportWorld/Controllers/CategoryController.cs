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
    }
}
