using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LogicLayer._const;
using DataLayer.Models;
using DbAccess.Core;
using LogicLayer._GenericActions;
using LogicLayer.Helpers;
using Microsoft.Toolkit.Parsers.Rss;

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

            if (!ShouldRefreshBlog(blog))
                return blog;

            string content = await _httpService.GetStringContent(blog.Url);
            IEnumerable<RssSchema> rss_schemas = FeedMethods.Parse(content);
            var result = blog.UpdatePosts(rss_schemas, _mapper);

            if (result == null)
            {
                AddError("Failed to update blog");
                return blog;
            }

            return blog;
        }

        private static bool ShouldRefreshBlog(Blog blog)
        {
            return blog.LastPostsRefreshDate.AddSeconds(RssConsts.UPDATE_BLOG_DELAY_S) < DateTime.UtcNow;
        }
    }
}
