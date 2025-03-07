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

namespace sportWorld.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller  
    {
		// Use ApplicationDbContext to map role to user
        private readonly ApplicationDbContext _db;
		public UserController(ApplicationDbContext db)
		{
            _db = db;
		}
		public IActionResult Index()
        {
			return View();
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
