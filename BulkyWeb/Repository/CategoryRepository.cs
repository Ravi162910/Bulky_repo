using BulkyWeb.Controllers;
using BulkyWeb.Data;
using BulkyWeb.Models;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    internal class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Category category)
        {
            dbSet.Update(category);
        }
        public IEnumerable<Category> GetAll()
        {
            return dbSet.ToList();
        }
    }
}
