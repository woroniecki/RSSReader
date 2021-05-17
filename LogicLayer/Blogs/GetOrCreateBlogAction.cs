using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;

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
            Blog blog = await _unitOfWork.BlogRepo.GetByUrl(url);
            
            if (blog == null)
            {
                string feed_content = await _httpService.GetStringContent(url);
                if (string.IsNullOrEmpty(feed_content))
                {
                    AddError("Can't get content under url.");
                    return null;
                }

                IEnumerable<RssSchema> parsed_feed = FeedMethods.Parse(feed_content);
                if (parsed_feed == null)
                {
                    AddError("Content under url is not rss format.");
                    return null;
                }

                blog = FeedMethods.CreateBlogObject(
                    url,
                    feed_content,
                    parsed_feed);

                FeedMethods.UpdateBlogPosts(blog, parsed_feed, _mapper);

                _unitOfWork.BlogRepo.AddNoSave(blog);
            }

            return blog;
        }
    }
}
