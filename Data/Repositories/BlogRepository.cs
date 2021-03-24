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
            : base(context.Blogs)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<UserPostData>> GetUserPostDatasAsync(int blogId, string userId)
        {
            return await _context.UserPostDatas
                .Include(x => x.Post)
                .Where(x => x.User.Id == userId && x.Post.Blog.Id == blogId)
                .ToListAsync();
        }
    }

    public interface IBlogRepository : IBaseRepository<Blog>
    {
        Task<bool> AddAsync(Blog blog);
        Task<IEnumerable<UserPostData>> GetUserPostDatasAsync(int blogId, string userId);
    }
}
