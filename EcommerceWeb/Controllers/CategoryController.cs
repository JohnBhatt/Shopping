using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> ObjCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id.Value == 0)
            {
                //Custom Error Page
                return NotFound();
            }
            //Find only wroks with Primary Key
            Category? catFromdb = _db.Categories.Find(id);
            //Alternative way of getting CategoryID from database
            //Category? catFromdb1 = _db.Categories.FirstOrDefault(u=>u.CategoryID==id); 
            //Category? catFromdb2 = _db.Categories.Where(u=>u.CategoryID==id).FirstOrDefault();

            if (catFromdb == null)
            {
                return NotFound();
            }
            return View(catFromdb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order and Name can't be same!!");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View();
        }


    }

}
