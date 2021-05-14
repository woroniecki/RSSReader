using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Posts;
using Dtos.UserPostData;
using LogicLayer.UserPostDatas;
using ServiceLayer._Commons;
using ServiceLayer._Runners;

namespace ServiceLayer.PostServices
{
    public class UpdateUserPostDataService : IUpdateUserPostDataService
    {
        private GetOrCreateUserPostDataAction _getOrCreateUserPostDataAction;
        private UpdateUserPostDataAction _updateUserPostDataAction;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public IImmutableList<ValidationResult> Errors
        {
            get
            {
                if (_getOrCreateUserPostDataAction != null && _getOrCreateUserPostDataAction.HasErrors)
                    return _getOrCreateUserPostDataAction.Errors;

                if (_updateUserPostDataAction != null && _updateUserPostDataAction.HasErrors)
                    return _updateUserPostDataAction.Errors;

                return _getOrCreateUserPostDataAction != null ? _getOrCreateUserPostDataAction.Errors :
                       _updateUserPostDataAction != null ? _updateUserPostDataAction.Errors :
                       null;
            }
        }

        public UpdateUserPostDataService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PostResponseDto> Update(UpdateUserPostDataRequestDto inData, int postId, string userId)
        {
            _getOrCreateUserPostDataAction = new GetOrCreateUserPostDataAction(userId, _unitOfWork);

            var user_post_data = await _getOrCreateUserPostDataAction.ActionAsync(postId);

            if (user_post_data == null || _getOrCreateUserPostDataAction.HasErrors)
                return null;

            _updateUserPostDataAction = new UpdateUserPostDataAction(user_post_data, _unitOfWork);

            var runner = new RunnerWriteDb<UpdateUserPostDataRequestDto, bool>(_updateUserPostDataAction, _unitOfWork.Context);
            
            //HACK if it created or update blog runner will savechanges
            var result = await runner.RunActionAsync(inData);

            if (!result || runner.HasErrors)
                return null;

            //TODO this could be normlized
            PostResponseDto post_dto = _mapper.Map<Post, PostResponseDto>(user_post_data.Post);
            post_dto.Readed = user_post_data.Readed;
            post_dto.Favourite = user_post_data.Favourite;

            return post_dto;
        }
    }

    public interface IUpdateUserPostDataService : IValidatedService
    {
        Task<PostResponseDto> Update(UpdateUserPostDataRequestDto inData, int postId, string userId);
    }
}
