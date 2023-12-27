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
        private readonly IUnitOfWork _unitofWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _unitofWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;

        }

        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userID)
        {
            RoleManagementVM roleVM = new RoleManagementVM() { 
            ApplicationUser = _unitofWork.ApplicationUser.Get(u=>u.Id == userID, includeProperties:"Company"), 
                RoleList = _roleManager.Roles.Select(i=> new SelectListItem { 
            Text=i.Name,
            Value=i.Name,
            }),
            CompanyList = _unitofWork.Company.GetAll().Select(i=>new SelectListItem
            {
                Text=i.Name,
                Value=i.ID.ToString(),
            }),
            };
            roleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitofWork.ApplicationUser.Get(u => u.Id == userID)).GetAwaiter().GetResult().FirstOrDefault();

            return View(roleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string oldRole = _userManager.GetRolesAsync(_unitofWork.ApplicationUser.Get(u => u.Id == roleManagementVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitofWork.ApplicationUser.Get(u => u.Id == roleManagementVM.ApplicationUser.Id);

            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyID = roleManagementVM.ApplicationUser.CompanyID;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyID = null;
                }
                _unitofWork.ApplicationUser.Update(applicationUser);
                _unitofWork.Save();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult(); ;
            }
            else
            { 
            if(oldRole == SD.Role_Company && applicationUser.CompanyID != roleManagementVM.ApplicationUser.CompanyID)
                {
                    applicationUser.CompanyID = roleManagementVM.ApplicationUser.CompanyID;
                    _unitofWork.ApplicationUser.Update(applicationUser);
                    _unitofWork.Save();
                }
            }
            TempData["success"] = "User Profile updated successfully!!";
            return RedirectToAction("Index");
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
            List<ApplicationUser> objUserList = _unitofWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach(var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                if (user.Company == null) {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string id)
        {
            var userFromDb = _unitofWork.ApplicationUser.Get(u => u.Id == id);
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
            _unitofWork.ApplicationUser.Update(userFromDb);
            _unitofWork.Save();
            return Json(new {success = true, message="Lock/Unlock Successfull."});
        }

        #endregion
    }
}
