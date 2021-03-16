using Microsoft.Toolkit.Parsers.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RSSReader.Data
{
    public enum FeedUrlError
    {
        NONE,
        WRONG_URL,
        NO_FEED_CONTENT
    }
    public class FeedService : IFeedService
    {
        /// If url exist and can be parsed as feed return true
        public async Task<FeedUrlError> VerifyFeedUrl(string url)
        {
            string feed = await GetContent(url);
            if (string.IsNullOrEmpty(feed))
                return FeedUrlError.WRONG_URL;

            IEnumerable<RssSchema> feed_list = ParseFeed(feed);
            if (feed_list == null)
                return FeedUrlError.NO_FEED_CONTENT;

            return FeedUrlError.NONE;
        }

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
    }

    public interface IFeedService
    {
        Task<FeedUrlError> VerifyFeedUrl(string url);
        Task<string> GetContent(string url);
        IEnumerable<RssSchema> ParseFeed(string feed);
    }
}
