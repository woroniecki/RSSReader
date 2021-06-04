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
            if (_unitOfWork.Context.Entry(subscription).State != Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                if (subscription.Active)
                {
                    AddError("Subscription is already added.");
                    return false;
                }
            }
            
            subscription.Active = true;
            subscription.LastSubscribeDate = DateTime.UtcNow;
            return true;
        }
    }
}
