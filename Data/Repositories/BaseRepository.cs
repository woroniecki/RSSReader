using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private DbSet<T> _dbSet;
        private DataContext _context;

        public BaseRepository(DbSet<T> dbSet, DataContext context)
        {
            _dbSet = dbSet;
            _context = context;
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetByID(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> Remove(T entity)
        {
            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<T>> GetList(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }

    public interface IBaseRepository<T>
    {
        Task<T> Get(Expression<Func<T, bool>> predicate);
        Task<T> GetByID(int id);
        Task<List<T>> GetList(Expression<Func<T, bool>> predicate);
        Task<bool> Remove(T entity);
    }
}
