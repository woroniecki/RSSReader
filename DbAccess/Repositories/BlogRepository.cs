using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLayer.Code;
using DataLayer.Models;
using System.Linq;
using System.Collections.Generic;

namespace DbAccess.Repositories
{
    public class BlogRepository : BaseRepository<Blog>, IBlogRepository
    {
        public BlogRepository(DataContext context)
            : base(context.Blogs, context)
        {
        }

        /// <summary>
        /// Return blog as notracking with posts up to posts_amount
        /// </summary>
        /// <param name="url">url of blog to get</param>
        /// <param name="posts_amount">posts amount to attach to untruck blog</param>
        /// <returns>return untrack blog, which never should be tracked</returns>
        public async Task<Blog> GetByUrl(string url, int posts_amount)
        {
            var query = (from b in _context.Blogs
                         where b.Url == url
                         select new
                         {
                             Blog = b,
                             Posts = b.Posts
                                      .OrderByDescending(x => x.PublishDate)
                                      .Take(posts_amount)
                         });

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

        public async Task<List<Blog>> GetListWithPosts(int skip, int take)
        {
            return await _context.Blogs
                .Include(x => x.Posts)
                .Skip(skip).Take(take)
                .ToListAsync();
        }
    }

    public interface IBlogRepository : IBaseRepository<Blog>
    {
        Task<Blog> GetByUrl(string url, int posts_amount);
        Task<Blog> GetWithPosts(Expression<Func<Blog, bool>> predicate);
        Task<List<Blog>> GetListWithPosts(int skip, int take);
    }
}
