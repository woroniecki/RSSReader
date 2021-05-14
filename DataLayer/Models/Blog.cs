using System;
using System.Collections.Generic;

namespace DataLayer.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public DateTime LastPostsRefreshDate { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
