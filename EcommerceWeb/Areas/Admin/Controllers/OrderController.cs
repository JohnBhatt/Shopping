using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.DataAccess;
using Ecommerce.Utility;
using Ecommerce.Models;
using Ecommerce.DataAccess.Repository.IRepository;
using System.Diagnostics;
using Ecommerce.Models.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        public OrderController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderID) {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitofWork.OrderHeader.Get(u => u.Id == orderID, includeProperties: "ApplicationUser"),
                OrderDetail = _unitofWork.OrderDetail.GetAll(u => u.OrderHeaderID == orderID, includeProperties: "Product")
            };
            return View(orderVM);
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders = _unitofWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess); break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped); ;
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }


            return Json(new { data = objOrderHeaders });
        }

        #endregion
    }
}
