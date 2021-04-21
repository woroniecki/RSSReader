using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class UserPostDataRepository : BaseRepository<UserPostData>, IUserPostDataRepository
    {
        public UserPostDataRepository(DataContext context)
            : base(context.UserPostDatas, context)
        {
        }
        public async Task<UserPostData> GetWithPost(Expression<Func<UserPostData, bool>> predicate)
        {
            return await _context.UserPostDatas
                .Include(x => x.Post)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<UserPostData>> GetListWithPosts(Expression<Func<UserPostData, bool>> predicate)
        {
            return await _context.UserPostDatas
                    .Include(x => x.Post)
                    .Where(predicate)
                    .ToListAsync();
        }
    }

    public interface IUserPostDataRepository : IBaseRepository<UserPostData>
    {
        Task<UserPostData> GetWithPost(Expression<Func<UserPostData, bool>> predicate);
        Task<IEnumerable<UserPostData>> GetListWithPosts(Expression<Func<UserPostData, bool>> predicate);
    }
}
