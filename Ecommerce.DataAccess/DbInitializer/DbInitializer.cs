using Eccomerce.DataAccess.Data;
using Ecommerce.DataAccess.DbInitializer;
using Ecommerce.Models;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.DbInitilizer
{
    /// <summary>
    /// This program will be responsible for primary actions in database for the first run. 
    /// </summary>
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer
            (
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        { 
        _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            //Missing Migrations
            try { 
            if(_db.Database.GetPendingMigrations().Count()> 0)
                {
                     _db.Database.Migrate();
                }
            }
            catch (Exception ex) {
            }


            //Create Roles if they are already not present
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult(); ;
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult(); ;
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult(); ;

                //Create First Admin User as well, if Roles table is empty.

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "TestAdmin@example.com",
                    Email = "TestAdmin@example.com",
                    Name = "Test Admin",
                    PhoneNumber = "1234567890",
                    StreetAddress = "Paschim Vihar",
                    State = "Delhi",
                    City = "New Delhi",
                    PostalCode = "12345",
                }, "Admin@123").GetAwaiter().GetResult();
                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "TestAdmin@example.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
