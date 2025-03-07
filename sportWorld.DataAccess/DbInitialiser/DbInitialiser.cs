using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sportWorld.DataAccess.Data;
using sportWorld.Models;
using sportWorld.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sportWorld.DataAccess.DbInitialiser
{
	public class DbInitialiser : IDbInitialiser
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _db;

		public DbInitialiser(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_db = db;
		}
		public void Initialise()
		{
			// Add pending migrations
			try
			{
				if (_db.Database.GetPendingMigrations().Count() >= 0)
				{
					_db.Database.Migrate();
				}
			} 
			catch (Exception e) {}

			// Create roles if they are not yet created
			if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
			}

			// Create admin user
			_userManager.CreateAsync(new ApplicationUser()
			{
				UserName = "adminproduction@gmail.com",
				Email = "adminproduction@gmail.com",
				Name = "Admin Production",
				PhoneNumber = "0123456789",
				StreetAddress = "123 Street",
				City = "Melbourne",
				State = "NSW",
				PostalCode = "6996"
			}, "Admin1!").GetAwaiter().GetResult();

			var adminUser = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "adminproduction@gmail.com");
			_userManager.AddToRoleAsync(adminUser, SD.Role_Admin).GetAwaiter().GetResult();

			return;
		}
	}
}
