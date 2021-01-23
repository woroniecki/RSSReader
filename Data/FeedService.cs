using Microsoft.Toolkit.Parsers.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RSSReader.Data
{
    public class FeedService : IFeedService
    {
        public virtual async Task<string> GetFeed(string url)
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
        Task<string> GetFeed(string url);
        IEnumerable<RssSchema> ParseFeed(string feed);
    }
}
