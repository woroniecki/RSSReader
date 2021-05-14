using System;
using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.Subscriptions
{
    public class EnableSubscriptionAction :
        ActionErrors,
        IAction<Subscription, bool>
    {
        private IUnitOfWork _unitOfWork;

        public EnableSubscriptionAction(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool Action(Subscription subscription)
        {
            if (subscription.Active)
            {
                return false;
            }
            
            subscription.Active = true;
            subscription.LastSubscribeDate = DateTime.UtcNow;
            return true;
        }
    }
}
