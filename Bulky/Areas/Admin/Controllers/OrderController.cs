using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Bulky.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bulky.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _UnitOfWork;

		public OrderController(IUnitOfWork unitOfWork)
		{
			_UnitOfWork=unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
			OrderVM orderVM= new OrderVM() { 
				OrderHeader=_UnitOfWork.OrderHeaderRepository.Get(u=>u.Id==orderId,IncludeProperties:"ApplicationUser"),
				OrderDetails=_UnitOfWork.OrderDetailRepository.GetAll(u=>u.OrderHeaderId==orderId,IncludeProperties:"Product")
			};
            return View(orderVM);
        }

        #region APi_CALLS
        public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderList = _UnitOfWork.OrderHeaderRepository.GetAll(IncludeProperties: "ApplicationUser").ToList();
			switch(status)
			{
				case "pending":
					objOrderList = objOrderList.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
					break;
                case "inprocess":
                    objOrderList = objOrderList.Where(u => u.OrderStatus == SD.statusInProcess);
                    break;
                case "completed":
                    objOrderList = objOrderList.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderList = objOrderList.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
				default:
					break;
            }
			return Json(objOrderList);
		}
		#endregion

	}
}
