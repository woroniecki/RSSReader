﻿using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLayer.Code;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using DbAccess._const;

namespace DbAccess.Repositories
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(DataContext context)
            : base(context.Subscriptions, context)
        {
        }

        /// <summary>
        /// This query also counts unreaded posts and includes blog and group
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="blog">Should has joined posts, as it will count unreaded</param>
        /// <returns>Subscription with counted unreaded</returns>
        public async Task<Subscription> GetAndCountUnreaded(string userId, Blog blog)
        {
            var query = (from s in _context.Subscriptions
                                           .Include(x => x.Blog)
                                           .Include(x => x.Group)
                         where s.UserId == userId && s.BlogId == blog.Id
                         select new
                         {
                             Subscription = s,
                             UserPostDatas = s.UserPostDatas
                                              .OrderByDescending(x => x.Post.PublishDate)
                                              .Take(RssConsts.POSTS_PER_CALL)
                         });

            var result = await query.FirstOrDefaultAsync();

            if (result != null)
            {
                result.Subscription.UnreadedCount = CountUnreadedPosts(blog.Posts, result.UserPostDatas); ;

                return result.Subscription;
            }

            return null;
        }

        /// <summary>
        /// Simple query to get subscription
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="blogId"></param>
        /// <returns>Subscription without any joins</returns>
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
                                      .Take(RssConsts.POSTS_PER_CALL),
                             UserPostDatas = s.UserPostDatas
                                              .OrderByDescending(x => x.Post.PublishDate)
                                              .Take(RssConsts.POSTS_PER_CALL)
                         });

            var result = await query.ToListAsync();

            foreach (var data in result)
            {
                data.Subscription.UnreadedCount = CountUnreadedPosts(data.Posts, data.UserPostDatas);
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

        private int CountUnreadedPosts(IEnumerable<Post> posts, IEnumerable<UserPostData> user_post_datas)
        {
            int unreaded_amount = 0;
            //Go only thru already readed posts
            var filtered_user_post_datas = user_post_datas.Where(x => x.Readed);

            foreach (var post in posts)
            {
                //If post weren't readed increment counter
                if (filtered_user_post_datas.FirstOrDefault(x => x.Id == post.Id) == null)
                {
                    unreaded_amount++;
                }
            }
            return unreaded_amount;
        }
    }

    public interface ISubscriptionRepository : IBaseRepository<Subscription>
    {
        Task<Subscription> GetAndCountUnreaded(string user, Blog blog);
        Task<Subscription> GetByUserIdAndBlogId(string userId, int blogId);
        Task<IEnumerable<Subscription>> GetListByUserId(string userId);
        Task<Subscription> GetByIdWithUser(int id);
    }
}
