﻿using System.Collections.Generic;
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
    public class SubscriptionSetGroupService : ISubscriptionSetGroupService
    {
        private SetGroupOfSubscriptionAction _action;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public IImmutableList<ValidationResult> Errors => _action != null ? _action.Errors : null;

        public SubscriptionSetGroupService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionResponseDto> SetGroup(int subId, int groupId, string userId)
        {
            _action = new SetGroupOfSubscriptionAction(subId, userId, _unitOfWork);

            var runner = new RunnerWriteDbAsync<int, Subscription>(_action, _unitOfWork.Context);

            var result = await runner.RunActionAsync(groupId);
            if (result == null || runner.HasErrors)
                return null;

            var returned_dto = _mapper.Map<Subscription, SubscriptionResponseDto>(result);

            return returned_dto;
        }
    }

    public interface ISubscriptionSetGroupService : IValidatedService
    {
        Task<SubscriptionResponseDto> SetGroup(int subId, int groupId, string userId);
    }
}