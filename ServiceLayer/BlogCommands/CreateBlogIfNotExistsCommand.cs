using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using HtmlAgilityPack;
using LogicLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Parsers.Rss;
using ServiceLayer._Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.BlogCommands
{
    public class CreateBlogIfNotExistsCommand : ICommand
    {
        public string Url { get; set; }
    }

    public class CreateBlogIfNotExistsCommandHandler : IHandleCommand<CreateBlogIfNotExistsCommand>
    {
        private DataContext _context;
        private IHttpHelperService _httpService;
        private IMapper _mapper;

        public CreateBlogIfNotExistsCommandHandler(DataContext context, IHttpHelperService httpService, IMapper mapper)
        {
            _context = context;
            _httpService = httpService;
            _mapper = mapper;
        }

        public async Task Handle(CreateBlogIfNotExistsCommand command)
        {
            string url = command.Url;
            Blog blog = await GetBlogQuery(url);

            //Couldn't find blog with url from request
            if (blog == null)
            {
                var http_response = await GetContentUnderUrl(url);

                //Maybe url was written diffrently, try once again to get blog based on response request uri
                if (url != http_response.RequestUrl)
                {
                    blog = await GetBlogQuery(http_response.RequestUrl);
                    if (blog != null)
                        return;
                }

                //Parse and verify if content under url is rss content
                IEnumerable<RssSchema> parsed_feed = FeedMethods.Parse(http_response.Content);

                //If it's not parseable rss content, it is probably html
                //Try to find rss url inside html or create a general one
                if (parsed_feed == null)
                {
                    string feed_url = TryToFindOrCreateProperFeedUrl(url, http_response);

                    //Maybe it's possible to find exisitng blog under new url
                    blog = await GetBlogQuery(feed_url);
                    if (blog != null)
                        return;

                    //If still nothing just try to get content from new url, maybe this one is the correct one
                    http_response = await GetContentUnderUrl(url);

                    //If url was written diffrently, try once again to get blog based on response request uri
                    if (feed_url != http_response.RequestUrl)
                    {
                        blog = await GetBlogQuery(http_response.RequestUrl);
                        if (blog != null)
                            return;
                    }

                    //As even with new feed_url coudln't find blog, seems that it's not in DB
                    parsed_feed = FeedMethods.Parse(http_response.Content);

                    //If content under new url it's still not rss just drop action
                    if (parsed_feed == null)
                        throw new Exception($"Can't find rss content under {http_response.RequestUrl}.");
                }

                using (var tx = _context.Database.BeginTransaction())
                {
                    blog = new Blog(
                        http_response.RequestUrl,//to avaid duplicated blogs use url from response
                        http_response.Content,
                        parsed_feed);

                    blog.ImageUrl = await BlogIconMethods.GetHigherIconResolution(blog.ImageUrl, _httpService);

                    if (blog.UpdatePosts(parsed_feed, _mapper) == null)
                        throw new Exception($"Couldn't do over posts under {http_response.RequestUrl}");

                    _context.Blogs.Add(blog);

                    _context.SaveChanges();

                    await tx.CommitAsync();
                }
            }
        }

        private async Task<Blog> GetBlogQuery(string url)
        {
            return await _context.Blogs
                .Include(x => x.Posts)
                .Where(x => x.Url == url)
                .FirstOrDefaultAsync();
        }

        private async Task<HttpHelperService.HttpCallResponse> GetContentUnderUrl(string url)
        {
            var http_response = await _httpService.GetRssHttpResponse(url);

            if (http_response == null)
                throw new Exception($"Can't get content under url {url}.");

            return http_response;
        }

        private string TryToFindOrCreateProperFeedUrl(string url, HttpHelperService.HttpCallResponse http_response)
        {
            string found_in_body_url = GetUrlOfFeed(http_response);

            if (!string.IsNullOrEmpty(found_in_body_url))
            {
                //Try again with link taken from content
                try
                {
                    //Verify if found url is full executable url, not just path after abosolute url
                    Uri found_in_body_uri = new Uri(found_in_body_url);
                    return found_in_body_url;
                }
                catch
                {
                    //If operation failed, it's probably path so just merge it with absolute url
                    return new Uri(url).AbsoluteUri + found_in_body_url.TrimStart();
                }
            }

            //Return common link syntax if it couldn't find it in sources
            return new Uri(url).AbsoluteUri + "feed";
        }

        /// <summary>
        /// Search for url of feed inside html source
        /// </summary>
        /// <param name="html">Html source</param>
        /// <returns>Url of feed or null if couldn't find any</returns>
        string GetUrlOfFeed(HttpHelperService.HttpCallResponse html)
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
            catch (Exception ex)
            {
                //TODO
                Console.Write(ex);
            }
            return null;
        }
    }
}
