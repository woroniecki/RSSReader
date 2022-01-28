using DataLayer.Code;
using DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLayer._Commons;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace ServiceLayer._CQRS.Commands.Admin
{
    public class SetUserRoleCommand : ICommand
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }

    public class SetUserRoleCommandHandler : IHandleCommand<SetUserRoleCommand>
    {
        private DataContext _context;
        private UserManager<ApiUser> _userManager;

        public SetUserRoleCommandHandler(DataContext context, UserManager<ApiUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Handle(SetUserRoleCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                var user = await _context.Users.Where(x => x.UserName == command.Username).FirstOrDefaultAsync();
                if (user == null)
                    throw ApiExceptions.General($"User {command.Username} doesn't exist.", Status400BadRequest);

                await user.SetRole(command.Role, _userManager);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }
    }
}
