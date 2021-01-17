using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RSSReader.Data
{
    public class PostRepository : IPostRepository
    {
        public static Expression<Func<Post, bool>> BY_POSTID(int id) => q => q.Id == id;
        public static Expression<Func<Post, bool>> BY_POSTURL(string url) => q => q.Url == url;

        private readonly DataContext _context;

        public PostRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Post> Get(Expression<Func<Post, bool>> predicate)
        {
            return await _context.Posts
                .FirstOrDefaultAsync(predicate);
        }
    }

    public interface IPostRepository
    {
        Task<Post> Get(Expression<Func<Post, bool>> predicate);
    }
}
