using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DataLayer.Code;
using DataLayer.Models;

namespace DbAccess.Repositories
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
