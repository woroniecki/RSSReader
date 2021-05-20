using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.Subscriptions
{
    public class SetGroupOfSubscriptionAction :
        ActionErrors,
        IActionAsync<int?, Subscription>
    {
        private int _subId;
        private Subscription _sub;
        private string _userId;
        private IUnitOfWork _unitOfWork;

        public SetGroupOfSubscriptionAction(Subscription sub, string userId, IUnitOfWork unitOfWork)
        {
            _sub = sub;
            _userId = userId;
            _unitOfWork = unitOfWork;
        }

        public async Task<Subscription> ActionAsync(int? groupId)
        {
            if (_sub == null)
            {
                AddError("Can't find subscription entity.");
                return null;
            }

            if (_sub.UserId != _userId)
            {
                AddError("Unauthorized.");
                return null;
            }

            //-1 is resetting group to null, so it's treaded as none group
            if (!groupId.HasValue || groupId == -1)
            {
                _sub.GroupId = null;
                _sub.Group = null;
            }
            else
            {
                var group = await _unitOfWork.GroupRepo.GetByIdWithUser(groupId.Value);
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

                _sub.GroupId = groupId;
                _sub.Group = group;
            }

            return _sub;
        }
    }
}
