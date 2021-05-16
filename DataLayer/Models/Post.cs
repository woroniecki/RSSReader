using System;

namespace DataLayer.Models
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
        public string Url { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
