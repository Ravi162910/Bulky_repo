using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository;
using Models;

namespace BulkyWeb.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
    }
}
