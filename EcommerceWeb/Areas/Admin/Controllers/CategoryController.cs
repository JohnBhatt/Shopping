using Eccomerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]

    //To Authorize the entire page for selected roles only, we are using Authorize Tag before controller definition, if we need to control particular action like Create/Delete, we can write this tag just before their definition. 

    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitofWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> ObjCategoryList = _unitofWork.Category.GetAll().ToList();
            return View(ObjCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order and Name can't be same!!");
            }
            if (ModelState.IsValid)
            {
                _unitofWork.Category.Add(obj);
                _unitofWork.Save();
                TempData["success"] = "Category created successfully!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int? ID)
        {
            if (ID == null || ID == 0)
            {
                return NotFound();
            }
            Category? catFromdb = _unitofWork.Category.Get(u => u.ID == ID);

            if (catFromdb == null)
            {
                return NotFound();
            }
            return View(catFromdb);
        }

        [HttpPost]
        public IActionResult Edit(Category catFromdb)
        {
            if (catFromdb.Name == catFromdb.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order and Name can't be same!!");
            }
            if (ModelState.IsValid)
            {
                _unitofWork.Category.Update(catFromdb);
                _unitofWork.Save();
                TempData["success"] = "Category Updated successfully!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        //public IActionResult Delete(int? ID)
        //{
        //    if (ID == null || ID.Value == 0)
        //    {
        //        //Custom Error Page
        //        return NotFound();
        //    }
        //    //Find only wroks with Primary Key
        //    Category? catFromdb = _unitofWork.Category.Get(u => u.ID == ID);
        //    //Alternative way of getting CategoryID from database
        //    //Category? catFromdb1 = _db.Categories.FirstOrDefault(u=>u.CategoryID==id); 
        //    //Category? catFromdb2 = _db.Categories.Where(u=>u.CategoryID==id).FirstOrDefault();

        //    if (catFromdb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(catFromdb);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? ID)
        //{
        //    Category? catFromdb = _unitofWork.Category.Get(u => u.ID == ID);
        //    if (catFromdb == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitofWork.Category.Remove(catFromdb);
        //    _unitofWork.Save();
        //    TempData["success"] = "Category deleted successfully!";
        //    return RedirectToAction("Index", "Category");
        //}

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> objProductList = _unitofWork.Category.GetAll().ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var categoryToDelete = _unitofWork.Category.Get(u => u.ID == id);
            if (categoryToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
           
            _unitofWork.Category.Remove(categoryToDelete);
            _unitofWork.Save();

            List<Category> objProductList = _unitofWork.Category.GetAll().ToList();
            return Json(new { success = true, message = "Delete Successfull." });
        }
        #endregion
    }
}
