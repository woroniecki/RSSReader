using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLayer.Code;
using DataLayer.Models;

namespace DbAccess.Repositories
{
    public class UserRepository : BaseRepository<ApiUser>, IUserRepository
    {
        public UserRepository(DataContext context)
            : base(context.Users, context)
        {
        }

        public async Task<ApiUser> GetByID(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<ApiUser> GetByUsername(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<ApiUser> GetByEmail(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<ApiUser> GetWithRefreshTokens(Expression<Func<ApiUser, bool>> predicate)
        {
            return await _context.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<ApiUser> GetWithSubscriptions(Expression<Func<ApiUser, bool>> predicate)
        {
            return await _context.Users
                .Include(x => x.Subscriptions)
                .ThenInclude(x => x.Blog)
                .Include(x => x.Subscriptions)
                .ThenInclude(x => x.Group)
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate);
        }
    }

    public interface IUserRepository : IBaseRepository<ApiUser>
    {
        Task<ApiUser> GetByID(string id);
        Task<ApiUser> GetByUsername(string username);
        Task<ApiUser> GetByEmail(string email);
        Task<ApiUser> GetWithRefreshTokens(Expression<Func<ApiUser, bool>> predicate);
        Task<ApiUser> GetWithSubscriptions(Expression<Func<ApiUser, bool>> predicate);
    }
}
