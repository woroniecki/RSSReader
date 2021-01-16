using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Data
{
    public class BlogRepository : IBlogRepository
    {
        private readonly DataContext _context;

        public BlogRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Blog> GetByUrlAsync(string url)
        {
            return await _context.Blogs.FirstOrDefaultAsync(x => x.Url == url);
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

    public interface IBlogRepository
    {
        Task<Blog> GetByUrlAsync(string url);
        Task<bool> AddAsync(Blog blog);
        Task<IEnumerable<UserPostData>> GetUserPostDatasAsync(int blogId, string userId);
    }
}
