using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Models
{
    public class Post
    {
        public Post() { }
        public Post(string name, string url, Blog blog)
        {
            this.Name = name;
            this.Url = url;
            this.Blog = blog;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Blog Blog { get; set; }
    }
}
