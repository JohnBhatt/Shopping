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
                productVM.Product = _unitofWork.Product.Get(u => u.ID == id, includeProperties:"ProductImages");
                return View(productVM);
            }
            

        }

        [HttpPost]
        public IActionResult UpSert(ProductVM obj, List<IFormFile> files)
        {
           
            if (ModelState.IsValid)
            {
                if (obj.Product.ID == 0)
                {
                    _unitofWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitofWork.Product.Update(obj.Product);
                }
                _unitofWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                        string path = @"images\products\product-" + obj.Product.ID;
                        string productPath = Path.Combine(wwwRootPath, path);
                        if (!Directory.Exists(productPath))
                        {
                            Directory.CreateDirectory(productPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                        ProductImage productImage = new() { 
                        ImageUrl = @"\"+path+@"\"+fileName,
                        ProductID = obj.Product.ID,
                        };
                            if (obj.Product.ProductImages == null)
                            {
                                obj.Product.ProductImages = new List<ProductImage>();
                            }
                            obj.Product.ProductImages.Add(productImage);                        
                    }
                    _unitofWork.Product.Update(obj.Product);
                    _unitofWork.Save();
                }
                TempData["success"] = "Book Added/Updated successfully!";
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

        public IActionResult DeleteImage(int imageId)
        {
            var imageToDelete = _unitofWork.ProductImage.Get(u => u.Id == imageId);
            int productID = imageToDelete.ProductID;
            if(imageToDelete != null)
            {
                if(!string.IsNullOrEmpty(imageToDelete.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToDelete.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitofWork.ProductImage.Remove(imageToDelete);
                _unitofWork.Save();
                TempData["success"] = "Deleted Successfully!";
            }
            return RedirectToAction(nameof(UpSert), new { id = productID });
        }

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
            string path = @"images\products\product-" + id;
            string productPath = Path.Combine(_webHostEnvironment.WebRootPath, path);
            if (Directory.Exists(productPath))
            {
                string[] filePaths = Directory.GetFiles(productPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(productPath);
            }


                //var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageURL.TrimStart('\\'));
                //if (System.IO.File.Exists(oldImagePath))
                //{
                //    System.IO.File.Delete(oldImagePath);
                //}
                _unitofWork.Product.Remove(productToDelete);
            _unitofWork.Save();

            List<Product> objProductList = _unitofWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {success = true, message="Delete Successfull."});
        }

        #endregion
    }
}
