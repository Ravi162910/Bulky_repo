using BulkyWeb.Models;
using Models;
using Models.Models;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetailobj);
        void Save();
    }
}
