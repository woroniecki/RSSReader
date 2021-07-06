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
        private GetPostsListAction _getPostsListAction;
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

                if (_getPostsListAction != null && _getPostsListAction.HasErrors)
                    return _getPostsListAction.Errors;

                if (_getUserPostDataListAction != null && _getUserPostDataListAction.HasErrors)
                    return _getUserPostDataListAction.Errors;

                return _updateBlogPostsAction != null ? _updateBlogPostsAction.Errors :
                       _getPostsListAction != null ? _getPostsListAction.Errors :
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

            _getPostsListAction = new GetPostsListAction(blogId, _unitOfWork);

            var posts = await _getPostsListAction.ActionAsync(page);

            if (_getPostsListAction.HasErrors)
                return null;

            var post_dtos = _mapper.Map<IEnumerable<Post>, IEnumerable<PostResponseDto>>(posts);

            //Add user data if user is logged in
            if (!string.IsNullOrEmpty(userId))
            {
                _getUserPostDataListAction = new GetUserPostDataListAction(userId, _unitOfWork);

                var user_post_datas = await _getUserPostDataListAction.ActionAsync(blogId);

                if (_getUserPostDataListAction.HasErrors)
                    return null;

                foreach (var post in post_dtos)
                {
                    post.UserData = new UserPostDataResponseDto();

                    var user_post_data = user_post_datas
                        .FirstOrDefault(x => x.Post.Id == post.Id);

                    if (user_post_data != null)
                    {
                        post.UserData.Readed = user_post_data.Readed;
                        post.UserData.Favourite = user_post_data.Favourite;
                    }
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
