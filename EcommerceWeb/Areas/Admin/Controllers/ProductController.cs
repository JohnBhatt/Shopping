using Eccomerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> ObjProductList = _unitofWork.Product.GetAll(includeProperties:"Category").ToList();
            //To Pass multiple object/model in view, we have to use viewbag. In this case, we are trying to push Product List and Category List - For Dropdown. 
            return View(ObjProductList);
        }
        public IActionResult UpSert(int? id) //Update+Insert
        {
            //Moved this code directly inside productVM below.
            //IEnumerable<SelectListItem> CategoryList = _unitofWork.Category.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.ID.ToString(),
            //});

            //ViewBag.CategoryList = CategoryList;
            // Using ViewData Dictionary Type
            //ViewData["CategoryList"] = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitofWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.ID.ToString(),
                }),
                Product = new Product()
            };
            //For Creating
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            //For Modifying
            else
            {
                productVM.Product = _unitofWork.Product.Get(u => u.ID == id);
                return View(productVM);
            }


        }

        [HttpPost]
        public IActionResult UpSert(ProductVM obj, IFormFile? file)
        {
            //if (obj.Product.Title == null || obj.Product.Title == "")
            //{
            //    return NotFound(); 
            //}

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    //On Update Command, if there is already file present.
                    if (!string.IsNullOrEmpty(obj.Product.ImageURL))
                    {
                        //Get the URL and Path of old file, Delete the Old file. We have to remove the backslash at start of image, hence TrimStart method.
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                    }
                    obj.Product.ImageURL = @"\images\product\" + fileName;
                }

                if (obj.Product.ID == 0)
                {
                    _unitofWork.Product.Add(obj.Product);
                    _unitofWork.Save();
                    TempData["success"] = "Book Added successfully!";
                }
                else
                {
                    _unitofWork.Product.Update(obj.Product);
                    _unitofWork.Save();
                    TempData["success"] = "Book Updated successfully!";
                }
                return RedirectToAction("Index", "Product");


            }
            else
            {
                //Populating Dropdown in case of any error.
                obj.CategoryList = _unitofWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.ID.ToString(),
                });
                return View(obj);
            }
        }

        //public IActionResult Edit(int? ID)
        //{
        //    if (ID == null || ID == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? catFromdb = _unitofWork.Product.Get(u => u.ID == ID);

        //    if (catFromdb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(catFromdb);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product catFromdb)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitofWork.Product.Update(catFromdb);
        //        _unitofWork.Save();
        //        TempData["success"] = "Book Updated successfully!";
        //        return RedirectToAction("Index", "Product");
        //    }
        //    return View();
        //}

        //public IActionResult Delete(int? ID)
        //{
        //    if (ID == null || ID.Value == 0)
        //    {
        //        //Custom Error Page
        //        return NotFound();
        //    }
        //    //Find only wroks with Primary Key
        //    Product? catFromdb = _unitofWork.Product.Get(u => u.ID == ID);
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
        //    Product? catFromdb = _unitofWork.Product.Get(u => u.ID == ID);
        //    if (catFromdb == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitofWork.Product.Remove(catFromdb);
        //    _unitofWork.Save();
        //    TempData["success"] = "Book deleted successfully!";
        //    return RedirectToAction("Index", "Product");
        //}

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitofWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = _unitofWork.Product.Get(u => u.ID == id);
            if(productToDelete == null)
            {
                return Json(new {success=false, message= "Error while deleting"});
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageURL.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitofWork.Product.Remove(productToDelete);
            _unitofWork.Save();

            List<Product> objProductList = _unitofWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {success = true, message="Delete Successfull."});
        }

        #endregion
    }
}
