using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using LogicLayer._const;
using LogicLayer._GenericActions;
using LogicLayer.Helpers;
using Microsoft.Toolkit.Parsers.Rss;

namespace LogicLayer.Blogs
{
    public class GetOrCreateBlogAction :
        ActionErrors,
        IActionAsync<string, Blog>
    {
        private readonly IHttpHelperService _httpService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrCreateBlogAction(IHttpHelperService httpService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _httpService = httpService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Blog> ActionAsync(string url)
        {
            Blog blog = await GetBlog(url);

            if (blog == null)
            {
                var http_response = await _httpService.GetRssHttpResponse(url);
                if (http_response == null)
                {
                    AddError("Can't get content under url.");
                    return null;
                }

                //Maybe url was written diffrently, try once again to get blog based on response request uri
                if (url != http_response.RequestUrl)
                {
                    blog = await GetBlog(http_response.RequestUrl);
                    if (blog != null)
                        return blog;
                }

                IEnumerable<RssSchema> parsed_feed = FeedMethods.Parse(http_response.Content);
                if (parsed_feed == null)
                {
                    AddError("Content under url is not rss format.");
                    return null;
                }

                blog = FeedMethods.CreateBlogObject(
                    http_response.RequestUrl,//to avaid duplicated blogs use url from response
                    http_response.Content,
                    parsed_feed);

                blog.ImageUrl = await BlogIconMethods.GetHigherIconResolution(blog.ImageUrl, _httpService);

                FeedMethods.UpdateBlogPosts(blog, parsed_feed, _mapper);

                _unitOfWork.BlogRepo.AddNoSave(blog);
            }

            return blog;
        }

        private async Task<Blog> GetBlog(string url)
        {
            return await _unitOfWork.BlogRepo.GetByUrl(url, RssConsts.POSTS_PER_CALL);
        }
    }
}
