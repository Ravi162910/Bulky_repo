using BulkyWeb.Models;
using Models;

namespace DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company companyobj);
        void Save();
    }
}
