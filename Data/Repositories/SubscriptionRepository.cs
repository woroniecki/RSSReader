using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(DataContext context)
            : base(context.Subscriptions, context)
        {
        }

        public async Task<Subscription> GetByUserAndBlog(ApiUser user, Blog blog)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.User == user && x.Blog == blog);
        }
    }

    public interface ISubscriptionRepository : IBaseRepository<Subscription>
    {
        Task<Subscription> GetByUserAndBlog(ApiUser user, Blog blog);
    }
}
