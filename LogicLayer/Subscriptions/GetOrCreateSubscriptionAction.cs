using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.Subscriptions
{
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
            var subscription = await _unitOfWork.SubscriptionRepo.GetByUserIdAndBlog(_userId, blog);

            if (subscription == null)
            {
                subscription = new Subscription(_userId, blog);

                _unitOfWork.SubscriptionRepo.AddNoSave(subscription);
            }

            return subscription;
        }
    }
}
