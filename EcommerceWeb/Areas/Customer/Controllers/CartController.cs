using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Drawing.Text;
using System.Security.Claims;

namespace EcommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userID, includeProperties: "Product")
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQty(cart);
                ShoppingCartVM.OrderTotal += (cart.Price + cart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Increase(int cartID)
        {
            
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id== cartID);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int cartID)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartID);
            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
                cartFromDb.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int cartID)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartID);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQty(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
