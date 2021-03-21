﻿using Microsoft.Toolkit.Parsers.Rss;
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
    }

    public interface IFeedService
    {
        Task<string> GetContent(string url);
        IEnumerable<RssSchema> ParseFeed(string feed);
        Blog CreateBlogObject(string url, string feed, IEnumerable<RssSchema> parsedFeed);
    }
}
