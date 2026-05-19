using BulkyWeb.Models;
using Models;
using Models.Models;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeaderobj);
        void UpdateStatus(int id, string senderStatus, string? paymentStatus = null);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
    }
}
