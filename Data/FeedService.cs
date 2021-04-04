using AutoMapper;
using Microsoft.Toolkit.Parsers.Rss;
using RSSReader.Data.Repositories;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSSReader.Data
{
    public class FeedService : IFeedService
    {
        private IHttpService _httpService;
        private readonly IMapper _mapper;

        public FeedService(IHttpService httpService, IMapper mapper)
        {
            _httpService = httpService;
            _mapper = mapper;
        }
        /// Returns parsed collection of posts or null if can't parse
        public virtual IEnumerable<RssSchema> ParseFeed(string feed)
        {
            var parser = new RssParser();
            try
            {
                return parser.Parse(feed);
            }
            catch
            {
                return null;
            }
        }

        public Blog CreateBlogObject(string url, string feed, IEnumerable<RssSchema> parsedFeed)
        {
            Blog new_blog = new Blog() { Url = url };

            string title = "";
            string description = "";
            string image_url = "";

            XDocument xml = XDocument.Parse(feed, LoadOptions.PreserveWhitespace);

            XElement title_element = xml.Descendants()
                .FirstOrDefault(p => p.Name.LocalName == "title");
            if (title_element != null)
            {
                title = title_element.Value;
            }

            XElement description_element = xml.Descendants()
                .FirstOrDefault(p => p.Name.LocalName == "description");
            if (description_element != null)
            {
                description = description_element.Value;
            }
            else
            {
                XElement subtitle_element = xml.Descendants()
                    .FirstOrDefault(p => p.Name.LocalName == "subtitle");
                if (subtitle_element != null)
                {
                    description = subtitle_element.Value;
                }
            }

            XElement image_element = xml.Descendants()
                .FirstOrDefault(p => p.Name.LocalName == "image");
            if (image_element != null)
            {
                XElement imag_url_element = image_element.Descendants()
                    .FirstOrDefault(p => p.Name.LocalName == "url");
                if (imag_url_element != null)
                {
                    image_url = imag_url_element.Value;
                }
            }

            //if accidently found main bog info in item, than clear this info
            foreach(RssSchema schema in parsedFeed)
            {
                if (string.Equals(schema.Title, title))
                    title = "";

                if (string.Equals(schema.Summary, description))
                    description = "";

                if (string.Equals(schema.ImageUrl, image_url))
                    image_url = "";
            }

            if (string.IsNullOrEmpty(title))
                title = url;

            new_blog.Name = title;
            new_blog.Description = description;
            new_blog.ImageUrl = image_url;

            return new_blog;
        }

        /// Run refresh method, but first get content under blog feed url
        public async Task<IEnumerable<Post>> RefreshBlogPosts(Blog blog)
        {
            if (blog == null)
                return null;

            string content = await _httpService.GetStringContent(blog.Url);
            IEnumerable<RssSchema> rss_schemas = ParseFeed(content);

            if (rss_schemas == null)
                return null;

            return RefreshBlogPosts(blog, rss_schemas);
        }

        /// Refreshes posts:
        /// If there are any new posts it will create them and add to db
        /// It takes blog and compare posts of this blog with parsed feed list
        /// Return all added posts list
        public IEnumerable<Post> RefreshBlogPosts(Blog blog, IEnumerable<RssSchema> parsedFeed)
        {
            if (blog.Posts == null)
                return null;

            List<Post> new_posts = new List<Post>();

            foreach(var rss_schema in parsedFeed)
            {
                if(blog.Posts.FirstOrDefault(x => x.Name == rss_schema.Title) == null)
                {
                    Post post = _mapper.Map<Post>(rss_schema);
                    post.Blog = blog;
                    blog.Posts.Add(post);
                    new_posts.Add(post);
                }
            }

            return new_posts;
        }

        public bool ShouldRefreshBlog(Blog blog)
        {
            return blog.LastPostsRefreshDate.AddMinutes(2) < DateTime.Now;
        }
    }

    public interface IFeedService
    {
        IEnumerable<RssSchema> ParseFeed(string feed);
        Blog CreateBlogObject(string url, string feed, IEnumerable<RssSchema> parsedFeed);
        Task<IEnumerable<Post>> RefreshBlogPosts(Blog blog);
        IEnumerable<Post> RefreshBlogPosts(Blog blog, IEnumerable<RssSchema> parsedFeed);
        bool ShouldRefreshBlog(Blog blog);
    }
}
