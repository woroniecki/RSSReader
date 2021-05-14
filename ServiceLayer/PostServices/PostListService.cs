using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using DataLayer.Models;
using DbAccess.Core;
using Dtos.Posts;
using LogicLayer.Blogs;
using LogicLayer.Groups;
using LogicLayer.Helpers;
using LogicLayer.Posts;
using ServiceLayer._Commons;
using ServiceLayer._Runners;

namespace ServiceLayer.PostServices
{
    public class PostListService : IPostListService
    {
        private UpdateBlogPostsAction _updateBlogPostsAction;
        private GetAndUpdatePostsListAction _getAndUpdatePostsListAction;
        private GetUserPostDataListAction _getUserPostDataListAction;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private IHttpHelperService _httpService;

        public IImmutableList<ValidationResult> Errors
        {
            get
            {
                if (_updateBlogPostsAction != null && _updateBlogPostsAction.HasErrors)
                    return _updateBlogPostsAction.Errors;

                if (_getAndUpdatePostsListAction != null && _getAndUpdatePostsListAction.HasErrors)
                    return _getAndUpdatePostsListAction.Errors;

                if (_getUserPostDataListAction != null && _getUserPostDataListAction.HasErrors)
                    return _getUserPostDataListAction.Errors;

                return _updateBlogPostsAction != null ? _updateBlogPostsAction.Errors :
                       _getAndUpdatePostsListAction != null ? _getAndUpdatePostsListAction.Errors :
                       _getUserPostDataListAction != null ? _getUserPostDataListAction.Errors :
                       null;
            }
        }

        public PostListService(IMapper mapper, IUnitOfWork unitOfWork, IHttpHelperService httpService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpService = httpService;
        }

        public async Task<IEnumerable<PostResponseDto>> GetList(string userId, int blogId, int page)
        {
            _updateBlogPostsAction = new UpdateBlogPostsAction(_mapper, _httpService, _unitOfWork);

            var runner = new RunnerWriteDbAsync<int, Blog>(_updateBlogPostsAction, _unitOfWork.Context);

            var blog = await runner.RunActionAsync(blogId);
            
            if (blog == null || runner.HasErrors)
                return null;

            _getAndUpdatePostsListAction = new GetAndUpdatePostsListAction(userId, blogId, _unitOfWork);

            var posts = await _getAndUpdatePostsListAction.ActionAsync(page);

            if (_getAndUpdatePostsListAction.HasErrors)
                return null;

            _getUserPostDataListAction = new GetUserPostDataListAction(userId, _unitOfWork);

            var user_post_datas = await _getUserPostDataListAction.ActionAsync(blogId);

            if (_getUserPostDataListAction.HasErrors)
                return null;

            var post_dtos = _mapper.Map<IEnumerable<Post>, IEnumerable<PostResponseDto>>(posts);

            foreach (var user_post_data in user_post_datas)
            {
                PostResponseDto post_dto = post_dtos
                    .FirstOrDefault(x => x.Id == user_post_data.Post.Id);

                if (post_dto != null)
                {
                    post_dto.Readed = user_post_data.Readed;
                    post_dto.Favourite = user_post_data.Favourite;
                }
            }

            return post_dtos;
        }
    }

    public interface IPostListService : IValidatedService
    {
        Task<IEnumerable<PostResponseDto>> GetList(string userId, int blogId, int page);
    }
}
