using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public static Expression<Func<Group, bool>> BY_USER(ApiUser user) => q => q.User == user;

        private readonly DataContext _context;

        public GroupRepository(DataContext context)
            : base(context.Groups, context)
        {
            _context = context;
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

        public async Task<IEnumerable<Group>> GetAll(Expression<Func<Group, bool>> predicate)
        {
            return await _context.Groups
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AddAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Group group)
        {
            _context.Groups.Remove(group);
            return await _context.SaveChangesAsync() > 0;
        }
    }

    public interface IGroupRepository : IBaseRepository<Group>
    {
        Task<IEnumerable<Group>> GetAll(Expression<Func<Group, bool>> predicate);
        Task<bool> AddAsync(Group group);
    }
}
