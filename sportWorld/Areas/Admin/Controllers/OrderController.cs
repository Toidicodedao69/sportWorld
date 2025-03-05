using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;
using sportWorld.Models.ViewModels;
using sportWorld.Utility;

namespace sportWorld.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM orderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Details(int orderId)
		{
			orderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
			return View(orderVM);
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult UpdateOrderDetail()
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);

			// Update details
			orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = orderVM.OrderHeader.City;
			orderHeaderFromDb.State = orderVM.OrderHeader.State;
			orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
			
			if (!string.IsNullOrEmpty(orderVM.OrderHeader.CarrierNumber))
			{
				orderHeaderFromDb.CarrierNumber = orderVM.OrderHeader.CarrierNumber;
			}
			if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
			{
				orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
			}

			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
			;
		}


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

			switch (status)
			{
				case "pending":
					orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
					break;
				case "inprocess":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
					break;
				case "approved":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
					break;
				case "completed":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
					break;
				default:
					break;
			}
			return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
