using DataLayer.Code;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using ServiceLayer._Command;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.SubscriptionCommands
{
    public class SetGroupSubCommand : ICommand
    {
        public string UserId { get; set; }
        public int SubId { get; set; }
        public int? NewGroupId { get; set; }
    }

    public class SetGroupSubCommandHandler : IHandleCommand<SetGroupSubCommand>
    {
        private DataContext _context;

        public SetGroupSubCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task Handle(SetGroupSubCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                var sub = await _context.Subscriptions.FindAsync(command.SubId);
                Group group = await GetGroupToSet(command.NewGroupId);

                sub.SetGroup(command.UserId, group);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }

        private async Task<Group> GetGroupToSet(int? groupId)
        {
            Group group;

            if (!groupId.HasValue || groupId == -1)
            {
                group = null;
            }
            else
            {
                group = await _context.Groups
                    .Include(x => x.User)
                    .Where(x => x.Id == groupId.Value)
                    .FirstOrDefaultAsync();

                if (group == null)
                    throw new Exception("Cant find group entity.");
            }

            return group;
        }
    }
}
