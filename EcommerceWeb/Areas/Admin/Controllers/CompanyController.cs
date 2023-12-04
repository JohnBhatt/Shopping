using Eccomerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModels;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]

    //Area protected for Admin Role users.
    [Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitofWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> ObjCompanyList = _unitofWork.Company.GetAll().ToList();
            return View(ObjCompanyList);
        }
        public IActionResult UpSert(int? id) //Update+Insert
        {
            Company companyObj = new Company();
            if (id == null || id == 0)
            {
                return View(companyObj);
            }
            //For Modifying
            else
            {
                companyObj = _unitofWork.Company.Get(u => u.ID == id);
                return View(companyObj);
            }


        }

        [HttpPost]
        public IActionResult UpSert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.ID == 0)
                {
                    _unitofWork.Company.Add(obj);
                    _unitofWork.Save();
                    TempData["success"] = "Company Added successfully!";
                }
                else
                {
                    _unitofWork.Company.Update(obj);
                    _unitofWork.Save();
                    TempData["success"] = "Company Updated successfully!";
                }
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(obj);
            }
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitofWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objCompanyList = _unitofWork.Company.Get(u => u.ID == id);
            if(objCompanyList == null)
            {
                return Json(new {success=false, message= "Error while deleting"});
            }
            _unitofWork.Company.Remove(objCompanyList);
            _unitofWork.Save();
            return Json(new {success = true, message="Delete Successfull."});
        }

        #endregion
    }
}
