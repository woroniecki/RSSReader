using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLayer.Code;
using DataLayer.Models;
using System.Linq;
using DbAccess._const;

namespace DbAccess.Repositories
{
    public class BlogRepository : BaseRepository<Blog>, IBlogRepository
    {
        public BlogRepository(DataContext context)
            : base(context.Blogs, context)
        {
        }

        public async Task<Blog> GetByUrl(string url)
        {
            var query = (from b in _context.Blogs
                         where b.Url == url
                         select new
                         {
                             Blog = b,
                             Posts = b.Posts
                                      .OrderByDescending(x => x.PublishDate)
                                      .Take(RssConsts.POSTS_PER_CALL)
                         }).AsNoTracking();

            var result = await query.FirstOrDefaultAsync();

            if (result != null)
            {
                result.Blog.Posts = result.Posts.ToList();
                return result.Blog;
            }

            return null;
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
