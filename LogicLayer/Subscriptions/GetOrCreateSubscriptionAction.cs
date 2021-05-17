using System;
using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;
using LogicLayer._const;
using LogicLayer._GenericActions;

namespace LogicLayer.Subscriptions
{
    /// <summary>
    /// Action also counts unreaded posts, as this entity is response and it will be displayed on front
    /// </summary>
    public class GetOrCreateSubscriptionAction :
        ActionErrors,
        IActionAsync<Blog, Subscription>
    {
        private string _userId;
        private IUnitOfWork _unitOfWork;

        public GetOrCreateSubscriptionAction(string userId, IUnitOfWork unitOfWork)
        {
            _userId = userId;
            _unitOfWork = unitOfWork;
        }

        public async Task<Subscription> ActionAsync(Blog blog)
        {
            var subscription = await _unitOfWork
                                        .SubscriptionRepo
                                        .GetAndCountUnreaded(_userId, blog, RssConsts.POSTS_PER_CALL);

            if (subscription == null)
            {
                subscription = new Subscription(_userId, blog);

                subscription.UnreadedCount = Math.Min(blog.Posts.Count, RssConsts.POSTS_PER_CALL);

                _unitOfWork.SubscriptionRepo.AddNoSave(subscription);
            }

            return subscription;
        }
    }
}
