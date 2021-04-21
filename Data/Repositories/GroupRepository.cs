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
    }

    public interface IGroupRepository : IBaseRepository<Group>
    {
        Task<IEnumerable<Group>> GetListByUser(ApiUser user);
    }
}
