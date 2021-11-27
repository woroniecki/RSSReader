using AutoMapper;
using DataLayer.Models;
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
            try
            {
                var parser = new RssParser();
                return parser.Parse(feed);
            }
            catch
            {
                return null;
            }
        }
    }
}
