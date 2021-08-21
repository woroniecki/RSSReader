using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Subscriptions;
using LogicLayer._const;
using LogicLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ServiceLayer.SubscriptionServices;
using Tests.Helpers;

namespace Tests.Services.SubscriptionServices
{
    [TestFixture]
    class SubscribeServiceTest
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
        public async Task Subscribe_CreatesNewSubscriptionAndBlog_SubscriptionResponseDto()
        {
            //ARRANGE
            string url_from_data = "http://blogprogramisty.net/feed/";

            var dto = new SubscribeRequestDto() 
            {
                BlogUrl = url_from_data + "///////"//to verify normalization of url
            };

            var user = new ApiUser()
            {
                Id = "0"
            };

            _context.Add(user);
            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponseFromFile(dto.BlogUrl, url_from_data, "../../../Data/feeddata.xml");
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );
            var start_time = DateTime.UtcNow;
            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            var created_sub = _context
                .Subscriptions
                .Include(x => x.Blog)
                .ThenInclude(x => x.Posts)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == url_from_data)
                .First();
            Assert.IsNotNull(created_sub);
            Assert.That(created_sub.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub.FirstSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(created_sub.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Url, Is.EqualTo(url_from_data));
            Assert.IsNull(result.UserData.GroupId);
            httpHelperService.Verify();

            //<--verify if posts updated-->
            Assert.That(result.UserData.UnreadedCount, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(created_sub.Blog.Posts.Count, Is.EqualTo(result.UserData.UnreadedCount));
            Assert.That(created_sub.Blog.LastPostsRefreshDate, Is.GreaterThanOrEqualTo(start_time));
            Assert.IsNull(created_sub.GroupId);
        }

        [Test]
        public async Task Subscribe_CreatesNewSubscriptionAndBlogByWebsiteUrl_SubscriptionResponseDto()
        {
            //ARRANGE
            string url_from_data = "https://www.gry-online.pl/rss/news.xml";
            string website_url = "http://gry-online.pl/";

            var dto = new SubscribeRequestDto()
            {
                BlogUrl = website_url
            };

            var user = new ApiUser()
            {
                Id = "0"
            };

            _context.Add(user);
            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponseFromFile(url_from_data, url_from_data, "../../../Data/feeddata.xml")
                .GetRssHttpResponseFromFile(website_url, website_url, "../../../Data/website.html");
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );
            var start_time = DateTime.UtcNow;
            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            var created_sub = _context
                .Subscriptions
                .Include(x => x.Blog)
                .ThenInclude(x => x.Posts)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == url_from_data)
                .First();
            Assert.IsNotNull(created_sub);
            Assert.That(created_sub.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub.FirstSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(created_sub.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Url, Is.EqualTo(url_from_data));
            Assert.IsNull(result.UserData.GroupId);
            httpHelperService.Verify();

            //<--verify if posts updated-->
            Assert.That(result.UserData.UnreadedCount, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(created_sub.Blog.Posts.Count, Is.EqualTo(result.UserData.UnreadedCount));
            Assert.That(created_sub.Blog.LastPostsRefreshDate, Is.GreaterThanOrEqualTo(start_time));
            Assert.IsNull(created_sub.GroupId);
        }

        [Test]
        public async Task Subscribe_CreatesNewSubscriptionAndBlogByAddingFeedToUrl_SubscriptionResponseDto()
        {
            //ARRANGE
            string url_from_data = "http://gry-online.pl/feed";
            string website_url = "http://gry-online.pl";

            var dto = new SubscribeRequestDto()
            {
                BlogUrl = website_url
            };

            var user = new ApiUser()
            {
                Id = "0"
            };

            _context.Add(user);
            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponseFromFile(url_from_data, url_from_data, "../../../Data/feeddata.xml")
                .GetRssHttpResponseFromFile(website_url, website_url, "../../../Data/website_no_feed_url.html");
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );
            var start_time = DateTime.UtcNow;
            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            var created_sub = _context
                .Subscriptions
                .Include(x => x.Blog)
                .ThenInclude(x => x.Posts)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == url_from_data)
                .First();
            Assert.IsNotNull(created_sub);
            Assert.That(created_sub.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub.FirstSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(created_sub.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Url, Is.EqualTo(url_from_data));
            Assert.IsNull(result.UserData.GroupId);
            httpHelperService.Verify();

            //<--verify if posts updated-->
            Assert.That(result.UserData.UnreadedCount, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(created_sub.Blog.Posts.Count, Is.EqualTo(result.UserData.UnreadedCount));
            Assert.That(created_sub.Blog.LastPostsRefreshDate, Is.GreaterThanOrEqualTo(start_time));
            Assert.IsNull(created_sub.GroupId);
        }

        [Test]
        public async Task Subscribe_CreatesNewSubscriptionForExistingBlogAndAsignGroup_SubscriptionResponseDto()
        {
            //ARRANGE
            var user = new ApiUser();
            _context.Add(user);

            var group = new Group() { User = user };
            _context.Add(group);

            var dto = new SubscribeRequestDto()
            {
                BlogUrl = "www.exampleblog.com/feed",
                GroupId = group.Id
            };

            var blog = new Blog()
            {
                Url = dto.BlogUrl
            };
            _context.Add(blog);

            for (int i = 0; i < 12; i++)
            {
                var post = new Post() { BlogId = blog.Id };
                _context.Add(post);
            }

            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService();
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );

            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            var created_sub = _context
                .Subscriptions
                .Include(x => x.Blog)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == dto.BlogUrl)
                .First();
            Assert.IsNotNull(created_sub);
            Assert.That(created_sub.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub.Blog.Id, Is.EqualTo(blog.Id));
            Assert.That(created_sub.FirstSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(created_sub.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Url, Is.EqualTo(dto.BlogUrl));
            Assert.That(result.UserData.GroupId, Is.EqualTo(dto.GroupId));

            //<--count unreaded posts-->
            Assert.That(result.UserData.UnreadedCount, Is.EqualTo(RssConsts.POSTS_PER_CALL));//Max amount is RssConsts.POSTS_PER_CALL
            Assert.That(created_sub.Blog.Posts.Count, Is.GreaterThanOrEqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(created_sub.GroupId, Is.EqualTo(dto.GroupId));
        }

        [Test]
        public async Task Subscribe_CreatesNewSubscriptionForExistingBlogWithDiffrentUrl_SubscriptionResponseDto()
        {
            //ARRANGE
            string url_from_data = "http://blogprogramisty.net/feed/";

            var user = new ApiUser();
            _context.Add(user);

            var dto = new SubscribeRequestDto()
            {
                BlogUrl = url_from_data + "////////"
            };

            var blog = new Blog()
            {
                Url = url_from_data 
            };
            _context.Add(blog);

            for (int i = 0; i < 12; i++)
            {
                var post = new Post() { BlogId = blog.Id };
                _context.Add(post);
            }

            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponseFromFile(dto.BlogUrl, url_from_data, "../../../Data/feeddata.xml");
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );

            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            var created_sub = _context
                .Subscriptions
                .Include(x => x.Blog)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == url_from_data)
                .First();
            Assert.IsNotNull(created_sub);
            Assert.That(created_sub.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub.Blog.Id, Is.EqualTo(blog.Id));
            Assert.That(created_sub.FirstSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(created_sub.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Url, Is.EqualTo(url_from_data));

            //<--count unreaded posts-->
            Assert.That(result.UserData.UnreadedCount, Is.EqualTo(RssConsts.POSTS_PER_CALL));//Max amount is RssConsts.POSTS_PER_CALL
            Assert.That(created_sub.Blog.Posts.Count, Is.GreaterThanOrEqualTo(RssConsts.POSTS_PER_CALL));
        }

        [Test]
        public async Task Subscribe_EnableExistingSubscriptionAndAssignGroup_SubscriptionResponseDto()
        {
            //ARRANGE
            var user = new ApiUser();
            _context.Add(user);

            var group = new Group() { User = user };
            _context.Add(group);

            var dto = new SubscribeRequestDto()
            {
                BlogUrl = "www.exampleblog.com/feed",
                GroupId = group.Id
            };

            var blog = new Blog()
            {
                Id = 0,
                Url = dto.BlogUrl
            };
            _context.Add(blog);

            var post1 = new Post() { BlogId = blog.Id };
            var post2 = new Post() { BlogId = blog.Id };
            _context.Add(post1);
            _context.Add(post2);

            var sub = new Subscription(user.Id, blog)
            {
                Active = false
            };
            _context.Add(sub);

            var user_post_data = new UserPostData(post1, user, sub) { Readed = true };
            _context.Add(user_post_data);

            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService();
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );

            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            var created_sub = _context
                .Subscriptions
                .Include(x => x.Blog)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == dto.BlogUrl)
                .First();
            Assert.IsNotNull(created_sub);
            Assert.That(created_sub.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub.Blog.Id, Is.EqualTo(blog.Id));
            Assert.That(created_sub.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Url, Is.EqualTo(dto.BlogUrl));
            Assert.That(result.UserData.GroupId, Is.EqualTo(dto.GroupId));

            //<--count unreaded posts-->
            Assert.That(result.UserData.UnreadedCount, Is.EqualTo(1));
            Assert.That(created_sub.GroupId, Is.EqualTo(dto.GroupId));
            Assert.That(created_sub.GroupId, Is.EqualTo(dto.GroupId));
        }

        [Test]
        public async Task Subscribe_EnableExistingSubscriptionAndSetGroupAsNone_SubscriptionResponseDto()
        {
            //ARRANGE
            var group = new Group() { };
            _context.Add(group);

            var dto = new SubscribeRequestDto()
            {
                BlogUrl = "www.exampleblog.com/feed"
            };

            var user = new ApiUser()
            {
                Id = "0"
            };
            _context.Add(user);

            var blog = new Blog()
            {
                Id = 0,
                Url = dto.BlogUrl
            };
            _context.Add(blog);

            var post1 = new Post() { BlogId = blog.Id };
            var post2 = new Post() { BlogId = blog.Id };
            _context.Add(post1);
            _context.Add(post2);

            var sub = new Subscription(user.Id, blog)
            {
                Active = false,
                Group = group
            };
            _context.Add(sub);

            var user_post_data = new UserPostData(post1, user, sub) { Readed = true };
            _context.Add(user_post_data);

            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService();
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );

            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            var created_sub = _context
                .Subscriptions
                .Include(x => x.Blog)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == dto.BlogUrl)
                .First();
            Assert.IsNotNull(created_sub);
            Assert.That(created_sub.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub.Blog.Id, Is.EqualTo(blog.Id));
            Assert.That(created_sub.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Url, Is.EqualTo(dto.BlogUrl));
            Assert.That(result.UserData.GroupId, Is.EqualTo(dto.GroupId));

            //<--count unreaded posts-->
            Assert.That(result.UserData.UnreadedCount, Is.EqualTo(1));
            Assert.That(created_sub.GroupId, Is.EqualTo(dto.GroupId));
        }

        [Test]
        public async Task Subscribe_TryToEnableExistingSubscriptionWhichIsActive_Null()
        {
            //ARRANGE
            var dto = new SubscribeRequestDto()
            {
                BlogUrl = "www.exampleblog.com/feed"
            };

            var user = new ApiUser()
            {
                Id = "0"
            };
            _context.Add(user);

            var blog = new Blog()
            {
                Id = 0,
                Url = dto.BlogUrl
            };
            _context.Add(blog);

            var sub = new Subscription(user.Id, blog)
            {
                Active = true
            };
            _context.Add(sub);

            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService();
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );

            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Subscribe_CantGetGroup_Null()
        {
            //ARRANGE
            var dto = new SubscribeRequestDto()
            {
                BlogUrl = "www.exampleblog.com/feed",
                GroupId = 0
            };

            var user = new ApiUser()
            {
                Id = "0"
            };

            _context.Add(user);
            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService()
                .GetStringContentFromFile(dto.BlogUrl, "../../../Data/feeddata.xml");
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );
            var start_time = DateTime.UtcNow;
            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Subscribe_CantGetBlogDataFromUrl_Null()
        {
            //ARRANGE
            var dto = new SubscribeRequestDto()
            {
                BlogUrl = "www.exampleblog.com/feed"
            };

            var user = new ApiUser()
            {
                Id = "0"
            };

            _context.Add(user);
            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponse(dto.BlogUrl, null);
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                MapperHelper.GetNewInstance(),
                _unitOfWork,
                httpHelperService.Object
                );

            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            httpHelperService.Verify();
        }

        [Test]
        public async Task Subscribe_CantParseRssDataFromUrl_Null()
        {
            //ARRANGE
            var dto = new SubscribeRequestDto()
            {
                BlogUrl = "www.exampleblog.com/feed"
            };

            var user = new ApiUser()
            {
                Id = "0"
            };

            _context.Add(user);
            _context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService()
                .GetRssHttpResponse(
                dto.BlogUrl, 
                new HttpHelperService.HttpCallResponse() {
                    RequestUrl = dto.BlogUrl,
                    Content = "This is wrong feed data"
                });
            var startTime = DateTime.UtcNow;
            var service = new SubscribeService(
                MapperHelper.GetNewInstance(),
                _unitOfWork,
                httpHelperService.Object
                );

            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            httpHelperService.Verify();
        }
    }
}
