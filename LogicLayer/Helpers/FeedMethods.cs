using AutoMapper;
using DataLayer.Models;
using LogicLayer._const;
using Microsoft.Toolkit.Parsers.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogicLayer.Helpers
{
    public static class FeedMethods
    {
        public static IEnumerable<RssSchema> Parse(string feed)
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

        public static Blog CreateBlogObject(string url, string feed, IEnumerable<RssSchema> parsedFeed)
        {
            Blog new_blog = new Blog() { Url = url, Posts = new List<Post>() };

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
            foreach (RssSchema schema in parsedFeed)
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

        /// <summary>
        /// Update blog posts if required - time to next update is expired
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="httpProvider">Provider to take content from blog url</param>
        /// <param name="mapper">Required to map rssfeed to post model in db</param>
        /// <returns>IEnumerable of added posts, null if failed</returns>
        public static async Task<IEnumerable<Post>> UpdateBlogPostsIfRequired(Blog blog, IHttpHelperService httpProvider, IMapper mapper)
        {
            if (blog == null)
                return null;

            if (!ShouldRefreshBlog(blog))
                return null;

            string content = await httpProvider.GetStringContent(blog.Url);
            IEnumerable<RssSchema> rss_schemas = Parse(content);

            if (rss_schemas == null)
            {
                //TODO some log that it failed
                return null;
            }

            return UpdateBlogPosts(blog, rss_schemas, mapper);
        }

        private static bool ShouldRefreshBlog(Blog blog)
        {
            return blog.LastPostsRefreshDate.AddSeconds(RssConsts.UPDATE_BLOG_DELAY_S) < DateTime.UtcNow;
        }

        public static IEnumerable<Post> UpdateBlogPosts(Blog blog, IEnumerable<RssSchema> parsedFeed, IMapper mapper)
        {
            if (blog.Posts == null)
                return null;

            blog.LastPostsRefreshDate = DateTime.UtcNow;

            List<Post> new_posts = new List<Post>();

            foreach (var rss_schema in parsedFeed)
            {
                if (blog.Posts.FirstOrDefault(x => x.Name == rss_schema.Title) == null)
                {
                    Post post = mapper.Map<Post>(rss_schema);
                    post.Blog = blog;
                    blog.Posts.Add(post);
                    new_posts.Add(post);
                }
            }

            return new_posts;
        }
    }
}
