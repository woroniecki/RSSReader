using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Subscriptions;
using LogicLayer._const;
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
            var created_sub = _context
                .Subscriptions
                .Include(x => x.Blog)
                .ThenInclude(x => x.Posts)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == dto.BlogUrl)
                .First();
            Assert.IsNotNull(created_sub);
            Assert.That(created_sub.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub.FirstSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(created_sub.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Blog.Url, Is.EqualTo(dto.BlogUrl));
            Assert.IsNull(result.GroupId);
            httpHelperService.Verify();

            //<--verify if posts updated-->
            Assert.That(result.UnreadedCount, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(created_sub.Blog.Posts.Count, Is.EqualTo(result.UnreadedCount));
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
                Id = 0,
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
            Assert.That(result.Blog.Url, Is.EqualTo(dto.BlogUrl));
            Assert.That(result.GroupId, Is.EqualTo(dto.GroupId));

            //<--count unreaded posts-->
            Assert.That(result.UnreadedCount, Is.EqualTo(RssConsts.POSTS_PER_CALL));//Max amount is RssConsts.POSTS_PER_CALL
            Assert.That(created_sub.Blog.Posts.Count, Is.GreaterThanOrEqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(created_sub.GroupId, Is.EqualTo(dto.GroupId));
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
            Assert.That(result.Blog.Url, Is.EqualTo(dto.BlogUrl));
            Assert.That(result.GroupId, Is.EqualTo(dto.GroupId));

            //<--count unreaded posts-->
            Assert.That(result.UnreadedCount, Is.EqualTo(1));
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
            Assert.That(result.Blog.Url, Is.EqualTo(dto.BlogUrl));
            Assert.That(result.GroupId, Is.EqualTo(dto.GroupId));

            //<--count unreaded posts-->
            Assert.That(result.UnreadedCount, Is.EqualTo(1));
            Assert.That(created_sub.GroupId, Is.EqualTo(dto.GroupId));
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
                .GetStringContentReturnValue(dto.BlogUrl, null);
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
                .GetStringContentReturnValue(dto.BlogUrl, "This is wrong feed data");
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
