using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(DataContext context)
            : base(context.Posts, context)
        {
        }

        public async Task<IEnumerable<Post>> GetLatest(int blogId, int skipAmount, int amount)
        {
            return await _context.Posts
                .Where(x => x.Blog.Id == blogId)
                .OrderByDescending(x => x.PublishDate)
                .Skip(skipAmount)
                .Take(amount)
                .AsNoTracking()
                .ToListAsync();
        }
    }

    public interface IPostRepository : IBaseRepository<Post>
    {
        Task<IEnumerable<Post>> GetLatest(int blogId, int skipAmount, int amount);
    }
}
