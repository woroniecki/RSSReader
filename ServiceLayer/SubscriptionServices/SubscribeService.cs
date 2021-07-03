using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Blogs;
using Dtos.Subscriptions;
using LogicLayer.Blogs;
using LogicLayer.Helpers;
using LogicLayer.Subscriptions;
using ServiceLayer._Commons;
using ServiceLayer._Runners;

namespace ServiceLayer.SubscriptionServices
{
    public class SubscribeService : ISubscribeService
    {
        private GetOrCreateBlogAction _getOrCreateBlogAction;
        private GetOrCreateSubscriptionAction _getOrCreateSubscriptionAction;
        private EnableSubscriptionAction _enableSubscriptionAction;
        private SetGroupOfSubscriptionAction _setGroupOfSubscriptionAction;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private IHttpHelperService _httpService;

        public IImmutableList<ValidationResult> Errors {
            get
            {
                if (_getOrCreateBlogAction != null && _getOrCreateBlogAction.HasErrors)
                    return _getOrCreateBlogAction.Errors;

                if (_getOrCreateSubscriptionAction != null && _getOrCreateSubscriptionAction.HasErrors)
                    return _getOrCreateSubscriptionAction.Errors;

                if (_enableSubscriptionAction != null && _enableSubscriptionAction.HasErrors)
                    return _enableSubscriptionAction.Errors;

                if (_setGroupOfSubscriptionAction != null && _setGroupOfSubscriptionAction.HasErrors)
                    return _setGroupOfSubscriptionAction.Errors;

                if (_getOrCreateBlogAction != null)
                    return _getOrCreateBlogAction.Errors;

                if (_getOrCreateSubscriptionAction != null)
                    return _getOrCreateSubscriptionAction.Errors;

                if (_enableSubscriptionAction != null)
                    return _enableSubscriptionAction.Errors;

                if (_setGroupOfSubscriptionAction != null)
                    return _setGroupOfSubscriptionAction.Errors;

                return null;
            }
        }

        public SubscribeService(IMapper mapper, IUnitOfWork unitOfWork, IHttpHelperService httpService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpService = httpService;
        }

        public async Task<BlogResponseDto> Subscribe(SubscribeRequestDto inData, string userId)
        {
            _getOrCreateBlogAction = new GetOrCreateBlogAction(_httpService, _unitOfWork, _mapper);

            Blog blog = await _getOrCreateBlogAction.ActionAsync(inData.BlogUrl);

            if (blog == null || _getOrCreateBlogAction.HasErrors)
                return null;

            _getOrCreateSubscriptionAction = new GetOrCreateSubscriptionAction(userId, _unitOfWork);

            Subscription subscription = await _getOrCreateSubscriptionAction.ActionAsync(blog);

            if (subscription == null || _getOrCreateSubscriptionAction.HasErrors)
                return null;

            _setGroupOfSubscriptionAction = new SetGroupOfSubscriptionAction(subscription, userId, _unitOfWork);

            subscription = await _setGroupOfSubscriptionAction.ActionAsync(inData.GroupId);

            if (subscription == null || _setGroupOfSubscriptionAction.HasErrors)
                return null;

            _enableSubscriptionAction = new EnableSubscriptionAction(_unitOfWork);

            var runner = new RunnerWriteDb<Subscription, bool>(
                _enableSubscriptionAction,
                _unitOfWork.Context
                );

            //HACK
            // Runner will execute save db if subscription was added in previous section
            // or were enabled in EnableSubscriptionAction
            await runner.RunActionAsync(subscription);

            if (runner.HasErrors)
                return null;

            var returned_dto = _mapper.Map<BlogResponseDto>(subscription);

            return returned_dto;
        }
    }

    public interface ISubscribeService : IValidatedService
    {
        Task<BlogResponseDto> Subscribe(SubscribeRequestDto inData, string userId);
    }
}
