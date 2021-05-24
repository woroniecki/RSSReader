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
        private Subscription _sub;

        public DisableSubscriptionAction(string userId, IUnitOfWork unitOfWork, Subscription sub = null)
        {
            _userId = userId;
            _unitOfWork = unitOfWork;
            _sub = sub;
        }

        public async Task<Subscription> ActionAsync(int subscriptionId)
        {
            _sub ??= await _unitOfWork.SubscriptionRepo.GetByID(subscriptionId);

            if (_sub == null)
            {
                AddError("Entity doesn't exist.");
                return null;
            }

            if(_sub.UserId != _userId)
            {
                AddError("Unauthorized.");
                return null;
            }
            
            if (!_sub.Active)
            {
                AddError("Entity is already disabled.");
                return null;
            }

            _sub.Active = false;
            _sub.LastUnsubscribeDate = DateTime.UtcNow;

            return _sub;
        }
    }
}
