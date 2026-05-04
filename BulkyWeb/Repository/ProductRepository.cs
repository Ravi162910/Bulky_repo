
using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Product product)
        {
            var objfromDB = _db.Products.FirstOrDefault(u => u.Id == product.Id);
            if (objfromDB != null)
            {
                objfromDB.Title = product.Title;
                objfromDB.Description = product.Description;
                objfromDB.ISBN = product.ISBN;
                objfromDB.Price = product.Price;
                objfromDB.ListPrice = product.ListPrice;
                objfromDB.Price50 = product.Price50;
                objfromDB.Price100 = product.Price100;
                objfromDB.CategoryID = product.CategoryID;
                objfromDB.Author = product.Author;
                if (product.ImageUrl != null) 
                {
                    objfromDB.ImageUrl = product.ImageUrl;
                }
            }
        }
        public IEnumerable<Product> GetAll()
        {
            return dbSet.ToList();
        }
    }
}
