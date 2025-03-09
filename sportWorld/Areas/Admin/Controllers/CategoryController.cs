using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Utility;

namespace sportWorld.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // Only Admin has access 
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> CategoryList = _unitOfWork.Category.GetAll().ToList();
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
                _unitOfWork.Category.Add(category); // The EF Core will keep track of change to db
                _unitOfWork.Save(); // Changes will only be executed on db when calling SaveChanges()
                TempData["success"] = "Category created successfully!"; // Show notification for only 1 render
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
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            // Other method to get category from db
            // Category? categoryFromDb2 = _unitOfWork.Categories.FirstOrDefault(u => u.Id == id);
            // Category? categoryFromDb3 = _unitOfWork.Categories.Where(u=>u.Id == id).FirstOrDefault();

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
                _unitOfWork.Category.Update(category); // The EF Core will keep track of change to db
                _unitOfWork.Save(); // Changes will only be executed on db when calling SaveChanges()
                TempData["success"] = "Category updated successfully!"; // Show notification for only 1 render
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
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")] // Specify actions when the form is submitted 
        public IActionResult DeletePOST(int? id) // This action method's name is still "Delete" as specified in ActionName annotation
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully!"; // Show notification for only 1 render
            return RedirectToAction("Index");
        }

        #region 
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return Json(new { data = categories });
        }
		#endregion

	}
}
