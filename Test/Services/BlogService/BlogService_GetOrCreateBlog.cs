using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Moq;
using NUnit.Framework;
using ServiceLayer.BlogServices;
using System;
using System.Threading.Tasks;
using Tests.Helpers;

namespace Tests.Services
{
    [TestFixture]
    class BlogService_GetOrCreateBlog
    {
        private DataContext _context;
        private UnitOfWork _unitOfWork;

        [SetUp]
        public void SetUp()
        {
            var options = InMemoryDb.CreateNewContextOptions();
            _context = new DataContext(options);
            _unitOfWork = new UnitOfWork(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Dispose();
        }

        [Test]
        public async Task Creates_new_blog_with_correct_url_with_unnormalize_url__Newly_created_blog_with_correct_url()
        {
            //ARRANGE
            string url_from_data = "http://blogprogramisty.net/feed/";
            string unnorm_url_from_data = url_from_data + "///////";//to verify normalization of url

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponseFromFile(unnorm_url_from_data, url_from_data, "../../../Data/feeddata.xml");
            var startTime = DateTime.UtcNow;
            var service = new BlogService(
                _context,
                httpHelperService.Object,
                MapperHelper.GetNewInstance()
            );

            //ACT
            var result = await service.GetOrCreateBlog(unnorm_url_from_data);

            //ASSERT
            httpHelperService.Verify();
            Assert.That(result.Url, Is.EqualTo(url_from_data));
            Assert.That(result.Posts.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task Creates_new_blog_by_finding_feed_url_in_website__Newly_created_blog_with_correct_url()
        {
            //ARRANGE
            string url_from_data = "https://www.gry-online.pl/rss/news.xml";
            string website_url = "http://gry-online.pl/";

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponseFromFile(url_from_data, url_from_data, "../../../Data/feeddata.xml")
                .GetRssHttpResponseFromFile(website_url, website_url, "../../../Data/website.html");
            var service = new BlogService(
                _context,
                httpHelperService.Object,
                MapperHelper.GetNewInstance()
            );

            //ACT
            var result = await service.GetOrCreateBlog(website_url);

            //ASSERT
            httpHelperService.Verify();
            Assert.That(result.Url, Is.EqualTo(url_from_data));
            Assert.That(result.Posts.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task Creates_new_blog_by_adding_feed_to_url_in_website__Newly_created_blog_with_correct_url()
        {
            //ARRANGE
            string url_from_data = "http://gry-online.pl/feed";
            string website_url = "http://gry-online.pl";

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponseFromFile(url_from_data, url_from_data, "../../../Data/feeddata.xml")
                .GetRssHttpResponseFromFile(website_url, website_url, "../../../Data/website_no_feed_url.html");
            var service = new BlogService(
                _context,
                httpHelperService.Object,
                MapperHelper.GetNewInstance()
            );

            //ACT
            var result = await service.GetOrCreateBlog(website_url);

            //ASSERT
            httpHelperService.Verify();
            Assert.That(result.Url, Is.EqualTo(url_from_data));
            Assert.That(result.Posts.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task Get_exisitng_blog_from_url__Returns_exisitngBlog()
        {
            //ARRANGE
            string url = "http://gry-online.pl";
            var blog = new Blog()
            {
                Url = url
            };
            _context.Add(blog);

            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService();
            var service = new BlogService(
                _context,
                httpHelperService.Object,
                MapperHelper.GetNewInstance()
            );

            //ACT
            var result = await service.GetOrCreateBlog(url);

            //ASSERT
            Assert.That(result.Url, Is.EqualTo(url));
        }

        [Test]
        public async Task Finds_exisitng_blog_by_using_url_from_website_content__Returns_exisitngBlog()
        {
            //ARRANGE
            int EXISTING_BLOG_ID = 2;
            string website_content_url = "http://gry-online.pl";
            string feed_url = "http://gry-online.pl/feed";
            var blog = new Blog()
            {
                Id = EXISTING_BLOG_ID,
                Url = feed_url
            };
            _context.Add(blog);

            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponseFromFile(website_content_url, website_content_url, "../../../Data/website_no_feed_url.html");
            var service = new BlogService(
                _context,
                httpHelperService.Object,
                MapperHelper.GetNewInstance()
            );

            //ACT
            var result = await service.GetOrCreateBlog(website_content_url);

            //ASSERT
            httpHelperService.Verify();
            Assert.That(result.Id, Is.EqualTo(EXISTING_BLOG_ID));
            Assert.That(result.Url, Is.EqualTo(feed_url));
        }
    }
}
