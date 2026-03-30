using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRespository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            internal DbSet<T> dbSet = _db.Set<T>();
        }

        void IRespository<T>.Add(T entity)
        {
            dbSet.Add(entity);
        }

        T IRespository<T>.Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        IEnumerable<T> IRespository<T>.GetAll()
        {
            IQueryable<T>query = dbSet;
            return query.ToList();
        }

        void IRespository<T>.Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        void IRespository<T>.RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}