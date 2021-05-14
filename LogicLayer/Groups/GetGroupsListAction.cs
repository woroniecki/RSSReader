using System.Collections.Generic;
using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.Groups
{
    public class GetGroupsListAction :
        ActionErrors,
        IActionAsync<string, IEnumerable<Group>>
    {
        private IUnitOfWork _unitOfWork;

        public GetGroupsListAction(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Group>> ActionAsync(string userId)
        {
            ApiUser user = await _unitOfWork.UserRepo.GetByID(userId);
            if (user == null)
            {
                AddError("Unauthorized.");
                return null;
            }

            return await _unitOfWork.GroupRepo.GetListByUser(user);
        }
    }
}
