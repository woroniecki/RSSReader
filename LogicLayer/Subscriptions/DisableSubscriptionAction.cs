using System;
using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.Subscriptions
{
    public class DisableSubscriptionAction :
        ActionErrors,
        IActionAsync<int, Subscription>
    {
        private string _userId;
        private IUnitOfWork _unitOfWork;

        public DisableSubscriptionAction(string userId, IUnitOfWork unitOfWork)
        {
            _userId = userId;
            _unitOfWork = unitOfWork;
        }

        public async Task<Subscription> ActionAsync(int subscriptionId)
        {
            var sub = await _unitOfWork.SubscriptionRepo.GetByID(subscriptionId);

            if (sub == null)
            {
                AddError("Entity doesn't exist.");
                return null;
            }

            if(sub.UserId != _userId)
            {
                AddError("Unauthorized.");
                return null;
            }
            
            if (!sub.Active)
            {
                AddError("Entity is already disabled.");
                return null;
            }

            sub.Active = false;
            sub.LastUnsubscribeDate = DateTime.UtcNow;

            return sub;
        }
    }
}
