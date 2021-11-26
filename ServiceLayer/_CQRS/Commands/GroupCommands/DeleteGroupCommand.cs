using DataLayer.Code;
using DataLayer.Models;
using Dtos.Groups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.GroupCommands
{
    public class DeleteGroupCommand : ICommand
    {
        public string UserId { get; set; }
        public RemoveGroupRequestDto Data { get; set; }
    }

    public class DeleteGroupCommandHandler : IHandleCommand<DeleteGroupCommand>
    {
        private DataContext _context;

        public DeleteGroupCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteGroupCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                ApiUser user = await _context.Users.FindAsync(command.UserId);

                if (user == null)
                    throw new Exception("Unauthorized.");

                Group group = await _context.Groups
                                  .Include(x => x.User)
                                  .Include(x => x.Subscriptions)
                                  .Where(x => x.Id == command.Data.GroupId &&
                                              x.User.Id == user.Id)
                                  .FirstOrDefaultAsync();
                
                if (group == null)
                    throw new Exception("Entity doesn't exist");

                foreach (var sub in group.Subscriptions)
                {
                    sub.SetGroup(command.UserId, group);
                    
                    if (command.Data.UnsubscribeSubscriptions && sub.Active)
                    {
                        sub.Disable(command.UserId);
                    }
                }

                _context.Remove(group);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }
    }
}
