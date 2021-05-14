using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;
using LogicLayer.Helpers;

namespace LogicLayer.Blogs
{
    public class UpdateBlogPostsAction :
        ActionErrors,
        IActionAsync<int, Blog>
    {
        private IMapper _mapper;
        private IHttpHelperService _httpService;
        private IUnitOfWork _unitOfWork;

        public UpdateBlogPostsAction(IMapper mapper, IHttpHelperService httpService, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _httpService = httpService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Blog> ActionAsync(int blogId)
        {
            var blog = await _unitOfWork.BlogRepo.GetWithPosts(x => x.Id == blogId);
            if (blog == null)
            {
                AddError("Can't find entity.");
                return null;
            }

            //HACK 
            //It would be much better if it wouldn't require to push httpService and mapper
            await FeedMethods.UpdateBlogPostsIfRequired(blog, _httpService, _mapper);

            return blog;
        }
    }
}
