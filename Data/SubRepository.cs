using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Data
{
    public class SubRepository : ISubRepository
    {
        private readonly DataContext _context;

        public SubRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync(Subscription blogSub)
        {
            await _context.Subscriptions.AddAsync(blogSub);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Subscription> GetByUserAndBlogAsync(ApiUser user, Blog blog)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.User == user && x.Blog == blog);
        }
        public async Task<Subscription> Get(int id)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }

    public interface ISubRepository
    {
        Task<Subscription> GetByUserAndBlogAsync(ApiUser user, Blog blog);
        Task<bool> AddAsync(Subscription blogSub);
        Task<Subscription> Get(int id);
    }
}
