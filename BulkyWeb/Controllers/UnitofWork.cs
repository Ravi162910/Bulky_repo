using BulkyWeb.Data;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class UnitofWork : IUnitofWork
    {
        private ApplicationDbContext _db;
        public UnitofWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
        }
        public ICategoryRepository Category { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
