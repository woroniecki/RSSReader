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
        public BlogRepository(DataContext context)
            : base(context.Blogs, context)
        {
        }

        public async Task<Blog> GetByUrl(string url)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Url == url);
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
        Task<Blog> GetByUrl(string url);
        Task<Blog> GetWithPosts(Expression<Func<Blog, bool>> predicate);
    }
}
