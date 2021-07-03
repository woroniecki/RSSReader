using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;
using Dtos.Subscriptions;
using LogicLayer._GenericActions;

namespace LogicLayer.Subscriptions
{
    public class UpdateSubscriptionAction :
        ActionErrors,
        IActionAsync<UpdateSubscriptionRequestDto, Subscription>
    {
        private string _userId;
        private int _subId;
        private IUnitOfWork _unitOfWork;

        public UpdateSubscriptionAction(string userId, int subId, IUnitOfWork unitOfWork)
        {
            _userId = userId;
            _subId = subId;
            _unitOfWork = unitOfWork;
        }

        public async Task<Subscription> ActionAsync(UpdateSubscriptionRequestDto dto)
        {
            var sub = await _unitOfWork.SubscriptionRepo.GetByIdWithBlog(_subId);

            if(sub == null)
            {
                AddError("Can't find entity.");
                return null;
            }

            if (sub.UserId != _userId)
            {
                AddError("Unauthorized.");
                return null;
            }

            if (dto.FilterReaded.HasValue)
            {
                sub.FilterReaded = dto.FilterReaded.Value;
            }

            return sub;
        }
    }
}
