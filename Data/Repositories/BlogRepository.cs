using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class BlogRepository : BaseRepository<Blog>, IBlogRepository
    {
        public static Expression<Func<Blog, bool>> BY_BLOGID(int id) => q => q.Id == id;
        public static Expression<Func<Blog, bool>> BY_BLOGURL(string url) => q => q.Url == url;

        private readonly DataContext _context;

        public BlogRepository(DataContext context)
            : base(context.Blogs, context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Blog> GetWithPosts(Expression<Func<Blog, bool>> predicate)
        {
            return await _context.Blogs
                .Include(x => x.Posts)
                .FirstOrDefaultAsync(predicate);
        }
    }

    public interface IBlogRepository : IBaseRepository<Blog>
    {
        Task<bool> AddAsync(Blog blog);
        Task<Blog> GetWithPosts(Expression<Func<Blog, bool>> predicate);
    }
}
