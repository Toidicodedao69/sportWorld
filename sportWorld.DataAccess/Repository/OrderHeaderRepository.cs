using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sportWorld.DataAccess.Data;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;


namespace sportWorld.DataAccess.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(OrderHeader orderHeader)
		{
			_db.OrderHeaders.Update(orderHeader);
		}

		void IOrderHeaderRepository.UpdateStatus(int id, string orderStatus, string? paymentStatus)
		{
			var orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);

			if (orderHeader != null)
			{
				orderHeader.OrderStatus = orderStatus;
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					orderHeader.PaymentStatus = paymentStatus;
				}
			}
		}

		void IOrderHeaderRepository.UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);

			if (!string.IsNullOrEmpty(sessionId))
			{
				orderHeader.SessionId = sessionId;
			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderHeader.PaymentIntentId = paymentIntentId;
				orderHeader.PaymentDate = DateTime.Now;
			}
		}
	}
}
