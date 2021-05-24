using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DataLayer.Code;
using DataLayer.Models;

namespace DbAccess.Repositories
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public GroupRepository(DataContext context)
            : base(context.Groups, context)
        {
        }

        public async Task<IEnumerable<Group>> GetListByUser(ApiUser user)
        {
            return await _context.Groups
                .Where(x => x.User == user)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Group> GetByIdAndUserId(int id, string userId)
        {
            return await _context.Groups
                .Include(x => x.Subscriptions)
                .Where(x => x.Id == id && x.User.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Group> GetUserIdAndByName(string userId, string name)
        {
            return await _context
                .Groups
                .Where(x => x.User.Id == userId && x.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task<Group> GetByIdWithUser(int id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }
    }

    public interface IGroupRepository : IBaseRepository<Group>
    {
        Task<IEnumerable<Group>> GetListByUser(ApiUser user);
        Task<Group> GetByIdAndUserId(int id, string userId);
        Task<Group> GetUserIdAndByName(string userId, string name);
        Task<Group> GetByIdWithUser(int id);
    }
}
