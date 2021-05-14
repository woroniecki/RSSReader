using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.Groups
{
    public class RemoveGroupAction :
        ActionErrors,
        IActionAsync<int, bool>
    {
        private string _requestingUserId;
        private IUnitOfWork _unitOfWork;

        public RemoveGroupAction(string requestingUserId, IUnitOfWork unitOfWork)
        {
            _requestingUserId = requestingUserId;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ActionAsync(int groupToRemoveId)
        {
            ApiUser user = await _unitOfWork.UserRepo.GetByID(_requestingUserId);
            if (user == null)
            {
                AddError("Unauthorized.");
                return false;
            }

            Group group = await _unitOfWork.GroupRepo.GetByIdAndUserId(groupToRemoveId, user.Id);
            if(group == null)
            {
                AddError("Entity doesn't exist");
                return false;
            }

            _unitOfWork.GroupRepo.RemoveNoSave(group);

            return true;
        }
    }
}
