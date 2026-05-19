
using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    internal class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(ShoppingCart cart)
        {
            _db.ShoppingCarts.Update(cart);
        }
    }
}
