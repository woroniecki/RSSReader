using DataLayer.Code;
using ServiceLayer._CQRS;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.SubscriptionCommands
{
    public class DisableSubCommand : ICommand
    {
        public string UserId { get; set; }
        public int SubId { get; set; }
    }

    public class DisableSubCommandHandler : IHandleCommand<DisableSubCommand>
    {
        private DataContext _context;

        public DisableSubCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task Handle(DisableSubCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                var sub = await _context.Subscriptions.FindAsync(command.SubId);

                sub.Disable(command.UserId);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }
    }
}
