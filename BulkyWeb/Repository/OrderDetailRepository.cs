
using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository.IRepository;
using Models;
using Models.Models;

namespace DataAccess.Repository
{
    internal class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(OrderDetail orderDetailobj)
        {
            dbSet.Update(orderDetailobj);
        }
        public IEnumerable<OrderDetail> GetAll()
        {
            return dbSet.ToList();
        }
    }
}
