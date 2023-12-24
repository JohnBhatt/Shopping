using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
          
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(productList);
        }
        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.ID == id, includeProperties: "Category"),
                Count = 1,
                ProductId = id
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userID;
            //Getting Shopping Cart values for same product for user. In this case, if customer adds more product into cart, new cart should not be generated, instead the value in database should be updated.
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userID && u.ProductId== cart.ProductId );
            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                //Even the below line is commented, the update command will work. That's because EntityFramework is tracking the object created on Line 44 and it knows we have added/updated values of Count, so at the time of _UnitOfWork.Save() command, the cartFromDb will be updated.
                //If we do not want this object to be tracked, we need to disable tracking in Repository. That will be IRepository file and we need to add bool tracked = false at the end of get command. 
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userID).Count());

            }
            else
            {
                _unitOfWork.ShoppingCart.Add(cart);
                _unitOfWork.Save();

                //Putting Object Value in Session.
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userID).Count());
            }
            TempData["Success"] = "Cart Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CategoryWise(int id)
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            //IEnumerable<Product> productList = _unitOfWork.Product.GetAll(u => u.CategoryID == id,includeProperties: "Category").ToList();
            //Product product = _unitOfWork.Product.Get(u=>u.CategoryID ==id, includeProperties: "Category");
            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
