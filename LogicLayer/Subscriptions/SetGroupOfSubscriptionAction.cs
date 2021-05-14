using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.Subscriptions
{
    public class SetGroupOfSubscriptionAction :
        ActionErrors,
        IActionAsync<int, Subscription>
    {
        private int _subId;
        private string _userId;
        private IUnitOfWork _unitOfWork;

        public SetGroupOfSubscriptionAction(int subId, string userId, IUnitOfWork unitOfWork)
        {
            _subId = subId;
            _userId = userId;
            _unitOfWork = unitOfWork;
        }

        public async Task<Subscription> ActionAsync(int groupId)
        {
            var sub = await _unitOfWork.SubscriptionRepo.GetByIdWithUser(_subId);
            if (sub == null)
            {
                AddError("Can't find subscription entity.");
                return null;
            }

            if (sub.UserId != _userId)
            {
                AddError("Unauthorized.");
                return null;
            }

            //-1 is resetting group to null, so it's treaded as none group
            if (groupId == -1)
            {
                sub.GroupId = null;
                sub.Group = null;
            }
            else
            {
                var group = await _unitOfWork.GroupRepo.GetByIdWithUser(groupId);
                if (group == null)
                {
                    AddError("Cant find group entity.");
                    return null;
                }

                if (group.User.Id != _userId)
                {
                    AddError("Unauthorized.");
                    return null;
                }

                sub.GroupId = groupId;
                sub.Group = group;
            }

            return sub;
        }
    }
}
