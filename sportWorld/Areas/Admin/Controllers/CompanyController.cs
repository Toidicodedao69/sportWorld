using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Utility;
using System.Collections;
using System.Drawing.Text;
using sportWorld.Models;

namespace sportWorld.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		public CompanyController(IUnitOfWork unitOfWork)
		{
            _unitOfWork = unitOfWork;
		}
		public IActionResult Index()
        {
            List<Company> companyList = _unitOfWork.Company.GetAll().ToList();

			return View(companyList);
        }
        public IActionResult Upsert(int? id)
        {
			Company company = new();
			if (id == 0 || id == null)
            {
                return View(company);
            }

			company = _unitOfWork.Company.Get(u => u.Id == id);
			return View(company);
        }
		[HttpPost]
		public IActionResult Upsert(Company company)
		{
			if (ModelState.IsValid)
			{
				if (company.Id == 0)
				{
					_unitOfWork.Company.Add(company);
					TempData["success"] = "Company created successfully!";
				}
				else
				{
					_unitOfWork.Company.Update(company);
					TempData["success"] = "Company updated successfully!";
				}
				_unitOfWork.Save();
				return RedirectToAction("Index");
			}
			else
			{
				return View(company);
			}
		}

		// API 
		#region
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> companyList = _unitOfWork.Company.GetAll().ToList();

			return Json(new { data = companyList });
		}
		[HttpDelete]
		public IActionResult Delete(int id)
		{
			var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);

			if (companyToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting company" });
			}

			_unitOfWork.Company.Remove(companyToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Company deleted successfully!" });
		}
		#endregion
	}
}
