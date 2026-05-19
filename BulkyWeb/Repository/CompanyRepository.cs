
using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    internal class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Company company)
        {
            dbSet.Update(company);
        }
        public IEnumerable<Company> GetAll()
        {
            return dbSet.ToList();
        }
    }
}
