using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;
using Dtos.Groups;
using LogicLayer._GenericActions;
using LogicLayer.Subscriptions;

namespace LogicLayer.Groups
{
    public class RemoveGroupAction :
        ActionErrors,
        IActionAsync<RemoveGroupRequestDto, bool>
    {
        private string _requestingUserId;
        private IUnitOfWork _unitOfWork;

        public RemoveGroupAction(string requestingUserId, IUnitOfWork unitOfWork)
        {
            _requestingUserId = requestingUserId;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ActionAsync(RemoveGroupRequestDto data)
        {
            ApiUser user = await _unitOfWork.UserRepo.GetByID(_requestingUserId);
            if (user == null)
            {
                AddError("Unauthorized.");
                return false;
            }

            Group group = await _unitOfWork.GroupRepo.GetByIdAndUserId(data.GroupId, user.Id);
            if(group == null)
            {
                AddError("Entity doesn't exist");
                return false;
            }

            foreach(var sub in group.Subscriptions)
            {
                var reset_group = new SetGroupOfSubscriptionAction(sub, _requestingUserId,_unitOfWork);
                await reset_group.ActionAsync(null);//If it's null it won't execute any async methods

                foreach(var error in reset_group.Errors)
                {
                    AddError("Can't reset group\n" + error.ErrorMessage);
                }
                if (HasErrors) return false;

                if (data.UnsubscribeSubscriptions && sub.Active)
                {
                    var disable_sub = new DisableSubscriptionAction(user.Id, _unitOfWork, sub);
                    await disable_sub.ActionAsync(-1);//As we pushed instance of subscription there is no need to call db to get it, thats why -1 id

                    foreach (var error in disable_sub.Errors)
                    {
                        AddError("Can't unsubscribe\n" + error.ErrorMessage);
                    }
                    if (HasErrors) return false;
                }
            }

            _unitOfWork.GroupRepo.RemoveNoSave(group);

            return true;
        }
    }
}
