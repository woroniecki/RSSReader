using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class UserRepository : BaseRepository<ApiUser>, IUserRepository
    {
        public static Expression<Func<ApiUser, bool>> BY_USERID(string id) => q => q.Id == id;
        public static Expression<Func<ApiUser, bool>> BY_USEREMAIL(string email) => q => q.Email == email;
        public static Expression<Func<ApiUser, bool>> BY_USERNAME(string username) => q => q.UserName == username;

        private readonly DataContext _context;

        public UserRepository(DataContext context)
            : base(context.Users)
        {
            _context = context;
        }

        public async Task<ApiUser> GetWithRefreshTokens(Expression<Func<ApiUser, bool>> predicate)
        {
            return await _context.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<ApiUser> GetWithSubscriptions(Expression<Func<ApiUser, bool>> predicate)
        {
            return await _context.Users
                .Include(x => x.Subscriptions)
                .ThenInclude(x => x.Blog)
                .FirstOrDefaultAsync(predicate);
        }
    }

    public interface IUserRepository : IBaseRepository<ApiUser>
    {
        Task<ApiUser> GetWithRefreshTokens(Expression<Func<ApiUser, bool>> predicate);
        Task<ApiUser> GetWithSubscriptions(Expression<Func<ApiUser, bool>> predicate);
    }
}
