using AutoMapper;
using Moq;
using NUnit.Framework;
using RSSReader.Data;
using RSSReader.Data.Repositories;
using RSSReader.Helpers;
using RSSReader.Models;
using RSSReader.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class FeedServiceTests
    {
        private Mock<IHttpService> _httpService;
        private IMapper _mapper;
        private FeedService _feedService;
        private string _url;

        [SetUp]
        public void SetUp()
        {
            _httpService = new Mock<IHttpService>();
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });
            _mapper = mapper.CreateMapper();
            _feedService = new FeedService(_httpService.Object, _mapper);
            _url = "www.url.com";
        }

        #region CreateBlogObject
        [Test]
        public void CreateBlogObject_Ok_CreatedBlogWithAllInfoFields()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata.xml"))
            {
                feed_data = r.ReadToEnd();
            }

            //ACT
            var feed_list = _feedService.ParseFeed(feed_data);
            var result = _feedService.CreateBlogObject(_url, feed_data, feed_list);

            //ASSERT
            Assert.IsFalse(string.IsNullOrEmpty(result.Name));
            Assert.IsFalse(string.Equals(_url, result.Name));//should take main title node from feed
            Assert.IsFalse(string.IsNullOrEmpty(result.Description));
            Assert.IsFalse(string.IsNullOrEmpty(result.ImageUrl));
        }

        [Test]
        public void CreateBlogObject_Ok_CreatedBlogDescriptionFromSubtitle()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata_descriptionfromsubtitle.xml"))
            {
                feed_data = r.ReadToEnd();
            }

            //ACT
            var feed_list = _feedService.ParseFeed(feed_data);
            var result = _feedService.CreateBlogObject(_url, feed_data, feed_list);

            //ASSERT
            Assert.IsFalse(string.Equals(_url, result.Name));//should take main title node from feed
            Assert.IsFalse(string.IsNullOrEmpty(result.Description));
        }
        #endregion

        #region CreateBlogObject
        [Test]
        public void RefreshBlogPosts_EmptyPostsList_AddPosts()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata_descriptionfromsubtitle.xml"))
            {
                feed_data = r.ReadToEnd();
            }
            var feed_list = _feedService.ParseFeed(feed_data);
            Blog blog = new Blog();
            blog.Posts = new List<Post>();

            //ACT
            var result = _feedService.RefreshBlogPosts(blog, feed_list);

            //ASSERT
            foreach (var tuple in feed_list.Zip(result, (rss_schema, post) => (rss_schema, post)))
            {
                Assert.That(tuple.rss_schema.Title, Is.EqualTo(tuple.post.Name));
                Assert.That(tuple.rss_schema.Summary, Is.EqualTo(tuple.post.Summary));
                Assert.That(tuple.rss_schema.Content, Is.EqualTo(tuple.post.Content));
                Assert.That(tuple.rss_schema.Author, Is.EqualTo(tuple.post.Author));
                Assert.That(tuple.rss_schema.ImageUrl, Is.EqualTo(tuple.post.ImageUrl));
                Assert.That(tuple.rss_schema.FeedUrl, Is.EqualTo(tuple.post.Url));
                Assert.That(tuple.rss_schema.PublishDate, Is.EqualTo(tuple.post.PublishDate));
                Assert.That(blog, Is.EqualTo(tuple.post.Blog));

                Assert.That(blog.Posts.Count(x => x == tuple.post), Is.EqualTo(1));
            }
            Assert.That(feed_list.Count(), Is.EqualTo(result.Count()));
        }

        [Test]
        public void RefreshBlogPosts_BlogPostsListIsEmpty_ReturnNullFails()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata_descriptionfromsubtitle.xml"))
            {
                feed_data = r.ReadToEnd();
            }
            var feed_list = _feedService.ParseFeed(feed_data);
            Blog blog = new Blog();

            //ACT
            var result = _feedService.RefreshBlogPosts(blog, feed_list);

            //ASSERT
            Assert.IsNull(result);
        }

        [Test]
        public void RefreshBlogPosts_BlogContainsPosts_NoNewPosts()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata_descriptionfromsubtitle.xml"))
            {
                feed_data = r.ReadToEnd();
            }
            var feed_list = _feedService.ParseFeed(feed_data);

            Blog blog = new Blog();
            blog.Posts = new List<Post>();

            foreach (var rss_schema in feed_list)
            {
                Post post = _mapper.Map<Post>(rss_schema);
                post.Blog = blog;
                blog.Posts.Add(post);
            }


            //ACT
            var result = _feedService.RefreshBlogPosts(blog, feed_list);

            //ASSERT
            Assert.That(result.Count(), Is.EqualTo(0));

        }

        [Test]
        public void RefreshBlogPosts_BlogContainsPosts_AddMissingPosts()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata_descriptionfromsubtitle.xml"))
            {
                feed_data = r.ReadToEnd();
            }
            var feed_list = _feedService.ParseFeed(feed_data);
            Blog blog = new Blog();
            blog.Posts = new List<Post>();

            int[] posts_contined_by_blog_ids = { 0, 2, 4 };
            List<Post> posts_contained_blog_list = new List<Post>();
            
            //Add already contained posts
            foreach(int rss_feed_id in posts_contined_by_blog_ids)
            {
                Post post = _mapper.Map<Post>(feed_list.ToList()[rss_feed_id]);
                post.Blog = blog;
                blog.Posts.Add(post);
                posts_contained_blog_list.Add(post);
            }

            //ACT
            var result = _feedService.RefreshBlogPosts(blog, feed_list);

            //ASSERT
            foreach(var rss_schema in feed_list)
            {
                if (posts_contained_blog_list.Count(x => x.Name == rss_schema.Title) > 0)
                    continue;

                Assert.That(
                        result.Count(x => x.Name == rss_schema.Title),
                        Is.EqualTo(1)
                    );
            }

            foreach (var post in posts_contained_blog_list)
            {
                Assert.That(
                        result.Count(x => x.Name == post.Name),
                        Is.EqualTo(0)
                    );
            }
        }

        [Test]
        public void RefreshBlogPosts_BlogContainsPosts_AllPostsAreNew()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata_descriptionfromsubtitle.xml"))
            {
                feed_data = r.ReadToEnd();
            }
            var feed_list = _feedService.ParseFeed(feed_data);
            Blog blog = new Blog();
            blog.Posts = new List<Post>();
            blog.Posts.Add(new Post());

            //ACT
            var result = _feedService.RefreshBlogPosts(blog, feed_list);

            //ASSERT
            foreach (var tuple in feed_list.Zip(result, (rss_schema, post) => (rss_schema, post)))
            {
                Assert.That(tuple.rss_schema.Title, Is.EqualTo(tuple.post.Name));
                Assert.That(tuple.rss_schema.Summary, Is.EqualTo(tuple.post.Summary));
                Assert.That(tuple.rss_schema.Content, Is.EqualTo(tuple.post.Content));
                Assert.That(tuple.rss_schema.Author, Is.EqualTo(tuple.post.Author));
                Assert.That(tuple.rss_schema.ImageUrl, Is.EqualTo(tuple.post.ImageUrl));
                Assert.That(tuple.rss_schema.FeedUrl, Is.EqualTo(tuple.post.Url));
                Assert.That(tuple.rss_schema.PublishDate, Is.EqualTo(tuple.post.PublishDate));
                Assert.That(blog, Is.EqualTo(tuple.post.Blog));

                Assert.That(blog.Posts.Count(x => x == tuple.post), Is.EqualTo(1));
            }
            Assert.That(feed_list.Count(), Is.EqualTo(result.Count()));
        }
        #endregion
    }
}
