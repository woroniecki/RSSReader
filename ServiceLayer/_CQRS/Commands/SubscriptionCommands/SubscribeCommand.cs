using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Subscriptions;
using HtmlAgilityPack;
using LogicLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Parsers.Rss;
using ServiceLayer._CQRS;
using ServiceLayer.BlogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.SubscriptionCommands
{
    public class SubscribeCommand : ICommand
    {
        public string UserId { get; set; }
        public SubscribeRequestDto Data { get; set; }

        private int SubscribedEntityId { get; set; }
        public int GetSubscribedEntityId() { return SubscribedEntityId; }
        public void SetSubscribedEntityId(int value) { SubscribedEntityId = value; }
    }

    public class SubscribeCommandHandler : IHandleCommand<SubscribeCommand>
    {
        private DataContext _context;
        private IBlogService _blogService;

        public SubscribeCommandHandler(
            DataContext context,
            IBlogService blogService
            )
        {
            _context = context;
            _blogService = blogService;
        }

        public async Task Handle(SubscribeCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                Blog blog = await _blogService.GetOrCreateBlog(command.Data.BlogUrl);

                Subscription subscription = await GetOrCreateSubscription(command, blog);

                await AssignGroupToSubscription(command, subscription);

                if (_context.Entry(subscription).State == EntityState.Detached)
                {
                    //If it's new entity add it to db
                    _context.Subscriptions.Add(subscription);
                }
                else
                {
                    //If it's not new entity verify, if it's not already subcribed
                    if (subscription.Active)
                        throw new Exception("Subscription is already added.");
                }

                subscription.Activate();

                _context.SaveChanges();

                await tx.CommitAsync();

                if (subscription != null)
                    command.SetSubscribedEntityId(subscription.Id);
            }
        }

        private async Task<Subscription> GetOrCreateSubscription(SubscribeCommand command, Blog blog)
        {
            var subscription = await _context.Subscriptions
                                .Where(x => x.UserId == command.UserId && x.BlogId == blog.Id)
                                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                subscription = new Subscription(command.UserId, blog);
            }

            return subscription;
        }

        private async Task AssignGroupToSubscription(SubscribeCommand command, Subscription subscription)
        {
            Group group = group = await _context.Groups
                                .Include(x => x.User)
                                .Where(x => x.Id == command.Data.GroupId)
                                .FirstOrDefaultAsync();

            subscription.SetGroup(command.UserId, group);
        }
    }
}
