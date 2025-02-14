using Microsoft.AspNetCore.Mvc;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;

namespace sportWorld.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> ProductList = _unitOfWork.Product.GetAll().ToList();
            return View(ProductList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost] // Specify actions when the form is submitted 
        public IActionResult Create(Product product)
        {
            //if (product.Name == product.DisplayOrder.ToString()) // Custom validation
            //{
            //    ModelState.AddModelError("name", "Product Name can not match Display Order");
            //}
            if (ModelState.IsValid) // Validate before adding to db
            {
                _unitOfWork.Product.Add(product); // The EF Core will keep track of change to db
                _unitOfWork.Save(); // Changes will only be executed on db when calling SaveChanges()
                TempData["success"] = "Product created successfully!"; // Show notification for only 1 render
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
