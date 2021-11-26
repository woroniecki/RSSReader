using DataLayer.Code;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using ServiceLayer._CQRS;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.GroupCommands
{
    public class AddGroupCommand : ICommand
    {
        public string UserId { get; set; }
        public string GroupName { get; set; }
        public Guid Guid { get; set; }
    }

    public class AddGroupCommandHandler : IHandleCommand<AddGroupCommand>
    {
        private DataContext _context;

        public AddGroupCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task Handle(AddGroupCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                ApiUser user = await _context.Users.FindAsync(command.UserId);
                if (user == null)
                    throw new Exception("Unauthorized.");

                Group group_with_the_same_name = await _context.Groups
                                  .Where(x => x.User.Id == user.Id &&
                                              x.Name == command.GroupName)
                                  .FirstOrDefaultAsync();

                if (group_with_the_same_name != null)
                    throw new Exception("Group with the same name already exists.");

                Group group = new Group(command.Guid, command.GroupName, user);
                _context.Add(group);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }
    }
}
