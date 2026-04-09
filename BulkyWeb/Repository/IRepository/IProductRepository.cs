using BulkyWeb.Models;
using DataAccess.Repository.IRepository;
using Models;

namespace BulkyWeb.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        void Save();
    }
}
