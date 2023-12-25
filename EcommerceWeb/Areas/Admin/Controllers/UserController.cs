using Eccomerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModels;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]

    //Area protected for Admin Role users.
    [Authorize(Roles = SD.Role_Admin)]

    public class UserController : Controller
    {
        //private readonly IUnitOfWork _unitofWork;
        //private readonly UserManager<IdentityUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        //public UserController(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        //{
        //    _unitofWork = unitOfWork;
        //    _roleManager = roleManager;
        //    _userManager = userManager;

        //}

        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;            
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement()
        {
            //string RoleID = _roleManager.first.
            return View();
        }



        #region API Calls
        [HttpGet]
//        public IActionResult GetAll()
//        {
//            List<ApplicationUser> objUserList = _unitofWork.ApplicationUser.GetAll().ToList();
////            var userRoles = _unitofWork.ApplicationUser.i 

//            return Json(new { data = objUserList });
//        }


        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u=>u.Company).ToList();
            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string id)
        {
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if(userFromDb== null)
            {
                return Json(new {success=false, message= "Error while locking/unlocking"});
            }

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                userFromDb.LockoutEnd = DateTime.Now;
            }
            else
            { 
            userFromDb.LockoutEnd = DateTime.Now.AddDays(30);
            }
            _db.SaveChanges();
            return Json(new {success = true, message="Lock/Unlock Successfull."});
        }

        #endregion
    }
}
