using Microsoft.Toolkit.Parsers.Rss;
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
        /// Returns content under url or null if url can't be reached
        public virtual async Task<string> GetContent(string url)
        {
            string feed = null;

            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync(url);
                }
                catch 
                {
                    return null;
                }
            }
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

            IEnumerable<XElement> titles = xml.Descendants("title");
            if (titles.FirstOrDefault() != null)
            {
                title = titles.First().Value;
            }

            IEnumerable<XElement> descriptions = xml.Descendants("description");
            if (descriptions.FirstOrDefault() != null)
            {
                description = descriptions.First().Value;
            }

            IEnumerable<XElement> images = xml.Descendants("image");
            if (images.FirstOrDefault() != null)
            {
                IEnumerable<XElement> image_urls = images.First().Descendants("url");
                if (image_urls.FirstOrDefault() != null)
                {
                    image_url = image_urls.First().Value;
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
    }

    public interface IFeedService
    {
        Task<string> GetContent(string url);
        IEnumerable<RssSchema> ParseFeed(string feed);
        Blog CreateBlogObject(string url, string feed, IEnumerable<RssSchema> parsedFeed);
    }
}
