using Dtos.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Blogs
{
    public class BlogResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public UserBlogDataDto UserData { get; set; }
    }

    public class UserBlogDataDto
    {
        public int SubId { get; set; }
        public DateTime FirstSubscribeDate { get; set; }
        public DateTime LastSubscribeDate { get; set; }
        public DateTime LastUnsubscribeDate { get; set; }
        public BlogResponseDto Blog { get; set; }
        public int? UnreadedCount { get; set; }
        public int? GroupId { get; set; }
        public bool FilterReaded { get; set; }
    }
}
