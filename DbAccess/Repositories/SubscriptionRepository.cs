using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLayer.Code;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace DbAccess.Repositories
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(DataContext context)
            : base(context.Subscriptions, context)
        {
        }

        public async Task<Subscription> GetByUserIdAndBlog(string userId, Blog blog)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.Blog == blog);
        }

        /// <summary>
        /// Return list of actie subscription with included blog and group related to user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Return list of actie subscription with included blog and group  related to user</returns>
        public async Task<IEnumerable<Subscription>> GetListByUserId(string userId)
        {
            return await _dbSet
                .Include(x => x.Blog)
                .Include(x => x.Group)
                .Where(x => x.UserId == userId && x.Active)
                .ToListAsync();
        }

        public async Task<Subscription> GetByIdWithUser(int id)
        {
            return await _context.Subscriptions
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }
    }

    public interface ISubscriptionRepository : IBaseRepository<Subscription>
    {
        Task<Subscription> GetByUserIdAndBlog(string user, Blog blog);
        Task<IEnumerable<Subscription>> GetListByUserId(string userId);
        Task<Subscription> GetByIdWithUser(int id);
    }
}
