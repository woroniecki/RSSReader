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
        public static Expression<Func<Subscription, bool>> BY_USERANDBLOG(ApiUser user, Blog blog) 
            => q => q.User == user && q.Blog == blog;

        public static Expression<Func<Subscription, bool>> BY_USER(ApiUser user)
            => q => q.User == user;

        private readonly DataContext _context;

        public SubscriptionRepository(DataContext context)
            : base(context.Subscriptions, context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync(Subscription blogSub)
        {
            await _context.Subscriptions.AddAsync(blogSub);
            return await _context.SaveChangesAsync() > 0;
        }
    }

    public interface ISubscriptionRepository : IBaseRepository<Subscription>
    {
        Task<bool> AddAsync(Subscription blogSub);
    }
}
