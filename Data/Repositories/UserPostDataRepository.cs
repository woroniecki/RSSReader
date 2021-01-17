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
        public static Expression<Func<UserPostData, bool>> BY_USERPOSTDATAPOSTANDUSER(ApiUser user, Post post) 
            => q => q.User.Id == user.Id && q.Post.Id == post.Id;

        private readonly DataContext _context;

        public UserPostDataRepository(DataContext context)
            : base(context.UserPostDatas)
        {
            _context = context;
        }
        public async Task<UserPostData> GetWithPost(Expression<Func<UserPostData, bool>> predicate)
        {
            return await _context.UserPostDatas
                .Include(x => x.Post)
                .FirstOrDefaultAsync(predicate);
        }
    }

    public interface IUserPostDataRepository : IBaseRepository<UserPostData>
    {
        Task<UserPostData> GetWithPost(Expression<Func<UserPostData, bool>> predicate);
    }
}
