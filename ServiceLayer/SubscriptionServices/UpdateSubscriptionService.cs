using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Blogs;
using Dtos.Subscriptions;
using LogicLayer.Subscriptions;
using ServiceLayer._Commons;
using ServiceLayer._Runners;

namespace ServiceLayer.SubscriptionServices
{
    public class UpdateSubscriptionService : IUpdateSubscriptionService
    {
        private UpdateSubscriptionAction _action;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public IImmutableList<ValidationResult> Errors => _action != null ? _action.Errors : null;

        public UpdateSubscriptionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BlogResponseDto> Update(int subId, string userId, UpdateSubscriptionRequestDto inData)
        {
            _action = new UpdateSubscriptionAction(userId, subId, _unitOfWork);

            var runner = new RunnerWriteDbAsync<UpdateSubscriptionRequestDto, Subscription>(_action, _unitOfWork.Context);

            var result = await runner.RunActionAsync(inData);

            if (result == null || runner.HasErrors)
            {
                return null;
            }

            var response_dto = _mapper.Map<BlogResponseDto>(result);

            return response_dto;
        }
    }

    public interface IUpdateSubscriptionService : IValidatedService
    {
        Task<BlogResponseDto> Update(int subId, string userId, UpdateSubscriptionRequestDto inData);
    }
}
