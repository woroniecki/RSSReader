using AutoMapper;
using Microsoft.Toolkit.Parsers.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

        public Blog() { }

        public Blog(string url, string feed, IEnumerable<RssSchema> parsedFeed)
        {
            Url = url;
            Posts = new List<Post>();

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

            Name = title;
            Description = description;
            ImageUrl = image_url;
        }

        public (int Added, int Deleted, int Left)? UpdatePosts(IEnumerable<RssSchema> rss_schemas, IMapper mapper)
        {
            if (rss_schemas == null)
            {
                //TODO some log that it failed
                return null;
            }

            List<Post> new_posts = MapPosts(rss_schemas, mapper);

            var sorted_posts = Posts.OrderByDescending(x => x.AddedDate).ToList();
            List<Post> posts_to_delete = GetPostsToDelete(new_posts, sorted_posts);

            List<Post> posts_to_add = GetPostsToAdd(new_posts, sorted_posts);

            posts_to_delete.ForEach(x => Posts.Remove(x));
            posts_to_add.ForEach(x => Posts.Add(x));

            LastPostsRefreshDate = DateTime.UtcNow;

            return (
                Added: posts_to_add.Count(),
                Deleted: posts_to_delete.Count(),
                Left: Posts.Count
            );
        }

       public static double UPDATE_BLOG_DELAY_S = new TimeSpan(0, 3, 0, 0).TotalSeconds;
       private bool ShouldRefreshBlog()
       {
           return LastPostsRefreshDate.AddSeconds(UPDATE_BLOG_DELAY_S) < DateTime.UtcNow;
       }

    private List<Post> GetPostsToAdd(List<Post> new_posts, List<Post> sorted_posts)
        {
            int add_all_below = new_posts.Count() - 1;
            add_all_below = GetIndexOfLastAlreadyAddedBlogFromNewest(new_posts, sorted_posts, add_all_below);

            List<Post> posts_to_add = new List<Post>();

            for (int i = add_all_below; i >= 0; i--)
            {
                posts_to_add.Add(new_posts[i]);
            }

            return posts_to_add;
        }

        private static int GetIndexOfLastAlreadyAddedBlogFromNewest(List<Post> new_posts, List<Post> sorted_posts, int add_all_below)
        {
            for (int i = 0; i < sorted_posts.Count(); i++)
            {
                for (int j = 0; j < new_posts.Count(); j++)
                {
                    if (sorted_posts[i].Name == new_posts[j].Name)
                    {
                        return j - 1;
                    }
                }
            }

            return add_all_below;
        }

        private List<Post> GetPostsToDelete(List<Post> new_posts, List<Post> sorted_posts)
        {
            //Find last post in db from rss
            int last_newest_post_index = new_posts.Count - 1;
            int remove_all_above = -1;
            for (int i = 0; i < sorted_posts.Count() && last_newest_post_index > 0; i++)
            {
                if (sorted_posts[i].Name == new_posts[last_newest_post_index].Name)
                {
                    remove_all_above = last_newest_post_index + 1;
                }
            }

            List<Post> posts_to_delete = new List<Post>();

            for (int i = remove_all_above + 1; i < sorted_posts.Count(); i++)
            {
                if(sorted_posts[i].FavouriteAmount <= 0)
                    posts_to_delete.Add(sorted_posts[i]);
            }

            return posts_to_delete;
        }

        private List<Post> MapPosts(IEnumerable<RssSchema> rss_schemas, IMapper mapper)
        {
            List<Post> new_posts = new List<Post>();

            int keep_order = 0;
            foreach (var rss_schema in rss_schemas)
            {
                Post post = mapper.Map<Post>(rss_schema);
                post.AddedDate = DateTime.UtcNow.AddSeconds(keep_order--);
                post.Blog = this;
                new_posts.Add(post);
            }

            return new_posts;
        }
    }
}
