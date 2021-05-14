using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Subscriptions;
using LogicLayer.Subscriptions;
using ServiceLayer._Commons;
using ServiceLayer._Runners;

namespace ServiceLayer.SubscriptionServices
{
    public class UnsubscribeService : IUnsubscribeService
    {
        private DisableSubscriptionAction _action;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public IImmutableList<ValidationResult> Errors => _action != null ? _action.Errors : null;

        public UnsubscribeService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionResponseDto> Unsubscribe(int subId, string userId)
        {
            _action = new DisableSubscriptionAction(userId, _unitOfWork);

            var runner = new RunnerWriteDbAsync<int, Subscription>(_action, _unitOfWork.Context);

            var result = await runner.RunActionAsync(subId);
            if (result == null || runner.HasErrors)
                return null;

            var returned_dto = _mapper.Map<Subscription, SubscriptionResponseDto>(result);

            return returned_dto;
        }
    }

    public interface IUnsubscribeService : IValidatedService
    {
        Task<SubscriptionResponseDto> Unsubscribe(int subId, string userId);
    }
}
