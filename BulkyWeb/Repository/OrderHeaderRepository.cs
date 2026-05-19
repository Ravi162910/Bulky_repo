
using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository.IRepository;
using Models;
using Models.Models;

namespace DataAccess.Repository
{
    internal class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(OrderHeader orderHeaderobj)
        {
            dbSet.Update(orderHeaderobj);
        }
        public IEnumerable<OrderHeader> GetAll()
        {
            return dbSet.ToList();
        }

        public void UpdateStatus(int id, string orderStatus, string paymentStatus) 
        {
            var orderfromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderfromDb != null)
            {
                orderfromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderfromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId) 
        {
            var orderfromdb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderfromdb != null) 
            {
                orderfromdb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId)) 
            {
                orderfromdb.PaymentIntentId = paymentIntentId;
                orderfromdb.PaymentDate = DateTime.Now;
            }
        }
    }
}
