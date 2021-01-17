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

        public BaseRepository(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
    }

    public interface IBaseRepository<T>
    {
        Task<T> Get(Expression<Func<T, bool>> predicate);
    }
}
