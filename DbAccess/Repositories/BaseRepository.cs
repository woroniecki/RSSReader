using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLayer.Code;

namespace DbAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected DbSet<T> _dbSet;
        protected DataContext _context;

        public BaseRepository(DbSet<T> dbSet, DataContext context)
        {
            _dbSet = dbSet;
            _context = context;
        }

        public async Task<T> GetByID(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> Remove(T entity)
        {
            RemoveNoSave(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public void RemoveNoSave(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> Add(T entity)
        {
            AddNoSave(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public void AddNoSave(T entity)
        {
            foreach(var entry in _context.ChangeTracker.Entries())
            {
                string log = $"{entry.Entity.GetType().Name} {entry.ToString()}  {entry.State.ToString()}";
                log += "";
            }

            _dbSet.Add(entity);
        }
    }

    public interface IBaseRepository<T>
    {
        Task<T> GetByID(int id);
        Task<bool> Remove(T entity);
        void RemoveNoSave(T entity);
        Task<bool> Add(T entity);
        void AddNoSave(T entity);
    }
}
