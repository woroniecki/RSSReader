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

        public async Task<Subscription> GetByUserIdAndBlogId(string userId, int blogId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.BlogId == blogId);
        }

        /// <summary>
        /// Return list of actie subscription with included blog and group related to user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Return list of actie subscription with included blog and group  related to user</returns>
        public async Task<IEnumerable<Subscription>> GetListByUserId(string userId)
        {
            var list = await _dbSet
                .Include(x => x.Blog)
                .ThenInclude(x => x.Posts)
                .Include(x => x.Group)
                .Include(x => x.UserPostDatas)
                .Where(x => x.UserId == userId && x.Active)
                .ToListAsync();

            list.ForEach(x => {
                x.UnreadedCount = x.Blog.Posts.Count() - x.UserPostDatas.Where(x => x.Readed).Count();
            });

            return list;
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
        Task<Subscription> GetByUserIdAndBlogId(string userId, int blogId);
        Task<IEnumerable<Subscription>> GetListByUserId(string userId);
        Task<Subscription> GetByIdWithUser(int id);
    }
}
