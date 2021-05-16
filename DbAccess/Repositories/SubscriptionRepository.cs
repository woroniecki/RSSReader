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
            var query = (from s in _context.Subscriptions
                                           .Include(x => x.Blog)
                                           .Include(x => x.Group)
                         where s.UserId == userId && s.Active
                         select new
                         {
                             Subscription = s,
                             Posts = s.Blog.Posts
                                      .OrderByDescending(x => x.PublishDate)
                                      .Take(10),
                             UserPostDatas = s.UserPostDatas
                                              .OrderByDescending(x => x.Post.PublishDate)
                                              .Take(10)
                         });

            var result = await query.ToListAsync();

            foreach (var data in result)
            {
                int unreaded_amount = 0;
                //Go only thru already readed posts
                var filtered_user_post_datas = data.UserPostDatas.Where(x => x.Readed);

                foreach (var post in data.Posts)
                {
                    //If post weren't readed increment counter
                    if (filtered_user_post_datas.FirstOrDefault(x => x.Id == post.Id) == null)
                    {
                        unreaded_amount++;
                    }
                }
                data.Subscription.UnreadedCount = unreaded_amount;
            }

            return result.Select(x => x.Subscription);
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
