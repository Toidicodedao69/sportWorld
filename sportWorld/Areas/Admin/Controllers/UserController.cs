using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Utility;
using System.Collections;
using System.Drawing.Text;
using sportWorld.Models;
using sportWorld.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using sportWorld.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace sportWorld.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller  
    {
		// Use ApplicationDbContext to map role to user
        private readonly ApplicationDbContext _db;
		private readonly UserManager<IdentityUser> _userManager;

		public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
		{
            _db = db;
			_userManager = userManager;
		}
		public IActionResult Index()
        {
			return View();
        }
		public IActionResult RoleManagement(string userId)
		{
			var roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

			UserVM userVM = new UserVM()
			{
				ApplicationUser = _db.ApplicationUsers.Include("Company").FirstOrDefault(u => u.Id == userId),
				RoleList = _db.Roles.ToList().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Name
				}),
				CompanyList = _db.Companies.ToList().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				})
			};

			userVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;

			return View(userVM);
		}
		[HttpPost]
		public IActionResult RoleManagement(UserVM userVM)
		{
			var roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userVM.ApplicationUser.Id).RoleId;
			var oldRole = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;
			var applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userVM.ApplicationUser.Id);

			applicationUser.Name = userVM.ApplicationUser.Name;

			if (oldRole != userVM.ApplicationUser.Role)
			{
				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(applicationUser, userVM.ApplicationUser.Role).GetAwaiter().GetResult();
			}

			if (userVM.ApplicationUser.Role == SD.Role_Company)
			{
				// Assign new company for company users
				applicationUser.CompanyId = userVM.ApplicationUser.CompanyId;
			}
			else
			{
				applicationUser.CompanyId = null;
			}

			_db.SaveChanges();
			TempData["success"] = "User role updated successfully!";
			return RedirectToAction(nameof(Index));
		}

		// API 
		#region
		[HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> userList = _db.ApplicationUsers.Include("Company").ToList();

			// Get userRoles and roles table from db to map role
			var userRolesTable = _db.UserRoles.ToList();
			var rolesTable = _db.Roles.ToList();

			foreach(var user in userList)
			{
				var roleId = userRolesTable.FirstOrDefault(u => u.UserId == user.Id).RoleId;
				user.Role = rolesTable.FirstOrDefault(u => u.Id == roleId).Name;

				if (user.Company == null)
				{
					user.Company = new Company()
					{
						Name = ""
					};
				}
			}

			return Json(new { data = userList });
		}
		[HttpPost]
		public IActionResult LockUnlock([FromBody]string id)
		{
			var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);

			if (userFromDb == null)
			{
				return Json(new { success = false, message = "User not found or existed" });
			}

			if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
			{
				// User locked -> unlock
				userFromDb.LockoutEnd = DateTime.Now;
				_db.SaveChanges();
				return Json(new { success = true, message = "Company unlocked successfully!" });
			}
			else
			{
				// User unlocked -> lock
				userFromDb.LockoutEnd = DateTime.Now.AddYears(100);
				_db.SaveChanges();
				return Json(new { success = true, message = "Company locked successfully!" });
			}

			
		}
		#endregion
	}
}
