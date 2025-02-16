using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Models.ViewModels;

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

            return View(productVM);
        }
        [HttpPost] // Specify actions when the form is submitted 
        public IActionResult Create(ProductVM productVM)
        {
            if (ModelState.IsValid) // Validate before adding to db
            {
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
