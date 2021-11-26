using DataLayer.Code;
using Dtos.Subscriptions;
using ServiceLayer._CQRS;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.SubscriptionCommands
{
    public class UpdateSubCommand : ICommand
    {
        public string UserId { get; set; }
        public int SubId { get; set; }
        public UpdateSubscriptionRequestDto UpdateData { get; set; }
    }

    public class UpdateSubCommandHandler : IHandleCommand<UpdateSubCommand>
    {
        private DataContext _context;

        public UpdateSubCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateSubCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                var sub = await _context.Subscriptions.FindAsync(command.SubId);

                sub.Update(command.UserId, command.UpdateData);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }
    }
}
