using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LogicLayer._const;
using DataLayer.Models;
using DbAccess.Core;
using HtmlAgilityPack;
using LogicLayer._GenericActions;
using LogicLayer.Helpers;
using Microsoft.Toolkit.Parsers.Rss;

namespace LogicLayer.Blogs
{
    public class GetOrCreateBlogAction :
        ActionErrors,
        IActionAsync<string, Blog>
    {
        public static string ErrorMsg_NoContent(string link) => $"Can't get content under url {link}.";
        public static string ErrorMsg_WrongUrlSyntax(string link) => $"Url syntax is not correct {link}.";
        public static string ErrorMsg_CantGetRssFormat(string link) => $"Can't find rss content under {link}.";
        public static string ErrorMsg_CantGetRssLink(string link) => $"Can't get rss link in {link}.";
        public const string ErrorMsg_BlogUpdateFail = "Update blog failed";

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
            return await RunRecursiveActionAsync(url, false);
        }

        public async Task<Blog> RunRecursiveActionAsync(string url, bool blockRecursion)
        {
            Blog blog = await GetBlog(url);

            if (blog == null)
            {
                var http_response = await _httpService.GetRssHttpResponse(url);
                if (http_response == null)
                {
                    AddError(ErrorMsg_NoContent(url));
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
                    string feed_url = GetFeedUrlFromHtml(http_response);

                    if (!blockRecursion)
                    {
                        Uri new_uri;
                        try
                        {
                            new_uri = new Uri(url);
                        }
                        catch
                        {
                            AddError(ErrorMsg_WrongUrlSyntax(url));
                            return null;
                        }

                        if (!string.IsNullOrEmpty(feed_url))
                        {
                            //Try again with link taken from content
                            string recursive_url;
                            try
                            {
                                Uri new_feed_uri = new Uri(feed_url);
                                recursive_url = feed_url;
                            }
                            catch
                            {
                                recursive_url = new_uri.AbsoluteUri + feed_url.TrimStart();
                            }
                            return await RunRecursiveActionAsync(recursive_url, true);
                        }
                        else
                        {
                            //Try to use common link if it couldn't find it in sources
                            try
                            {
                                return await RunRecursiveActionAsync(new_uri.AbsoluteUri + "feed", true);
                            }
                            catch
                            {
                                AddError(ErrorMsg_CantGetRssLink(url));
                                return null;
                            }
                        }
                    }

                    AddError(ErrorMsg_CantGetRssFormat(url));
                    return null;
                }

                blog = new Blog(
                    http_response.RequestUrl,//to avaid duplicated blogs use url from response
                    http_response.Content,
                    parsed_feed);

                blog.ImageUrl = await BlogIconMethods.GetHigherIconResolution(blog.ImageUrl, _httpService);

                if (blog.UpdatePosts(parsed_feed, _mapper) == null)
                {
                    AddError(ErrorMsg_BlogUpdateFail);
                    return null;
                }

                _unitOfWork.BlogRepo.AddNoSave(blog);
            }

            return blog;
        }

        string GetFeedUrlFromHtml(HttpHelperService.HttpCallResponse html)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html.Content);


                foreach (HtmlNode node in htmlDoc.DocumentNode
                                                 .Descendants("link")
                                                 .Where(x => "" != x.GetAttributeValue("type", "")))
                {
                    string type = node.GetAttributeValue("type", "");
                    switch (type)
                    {

                        case "application/rss+xml":
                        case "application/rdf+xml":
                        case "application/atom+xml":
                        case "application/xml":
                        case "text/xml":
                            string href = node.GetAttributeValue("href", "");
                            if ("" != href)
                                return href;

                            break;
                    }
                }
            } 
            catch(Exception ex)
            {
                //TODO
                Console.Write(ex);
            }
            return "";
        }

        private async Task<Blog> GetBlog(string url)
        {
            return await _unitOfWork.BlogRepo.GetByUrl(url, RssConsts.POSTS_PER_CALL);
        }
    }
}
