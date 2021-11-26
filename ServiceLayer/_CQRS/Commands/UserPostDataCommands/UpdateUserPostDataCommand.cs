using DataLayer.Code;
using DataLayer.Models;
using Dtos.UserPostData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.UserPostDataCommands
{
    public class UpdateUserPostDataCommand : ICommand
    {
        public string UserId { get; set; }
        public int PostId { get; set; }
        public UpdateUserPostDataRequestDto Data { get; set; }
    }

    public class UpdateUserPostDataCommandHandler : IHandleCommand<UpdateUserPostDataCommand>
    {
        private DataContext _context;

        public UpdateUserPostDataCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateUserPostDataCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                ApiUser user = await _context.Users.FindAsync(command.UserId);
                if (user == null)
                    throw new Exception("Unauthorized.");

                Post post = await _context.Posts.FindAsync(command.PostId);
                if (post == null)
                    throw new Exception("Can't find post entity.");

                Subscription sub = await _context.Subscriptions
                        .Where(x => x.UserId == command.UserId && x.BlogId == post.BlogId)
                        .FirstOrDefaultAsync();
                if (sub == null)
                    throw new Exception("Can't find sub entity.");

                UserPostData user_post_data = await _context.UserPostDatas
                        .Include(x => x.Post)
                        .Where(x => x.User == user && x.Post == post)
                        .FirstOrDefaultAsync();

                if (user_post_data == null)
                {
                    user_post_data = new UserPostData(post, user, sub);
                    _context.Add(user_post_data);
                }

                if (command.Data.Readed.HasValue)
                    user_post_data.SetReaded(command.Data.Readed.Value);

                if (command.Data.Favourite.HasValue)
                    user_post_data.SetFavourite(command.Data.Favourite.Value);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }
    }
}