using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Subscriptions;
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

            //ACT
            var result = await service.Subscribe(dto, user.Id);

            //ASSERT
            var created_sub_with_blog = _context
                .Subscriptions
                .Include(x => x.Blog)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == dto.BlogUrl)
                .First();
            Assert.IsNotNull(created_sub_with_blog);
            Assert.That(created_sub_with_blog.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub_with_blog.FirstSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(created_sub_with_blog.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Blog.Url, Is.EqualTo(dto.BlogUrl));
            Assert.IsNull(result.GroupId);
            httpHelperService.Verify();
        }

        [Test]
        public async Task Subscribe_CreatesNewSubscriptionForExistingBlog_SubscriptionResponseDto()
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

            var blog = new Blog()
            {
                Id = 0,
                Url = dto.BlogUrl
            };

            _context.Add(user);
            _context.Add(blog);
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
            var created_sub_with_blog = _context
                .Subscriptions
                .Include(x => x.Blog)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == dto.BlogUrl)
                .First();
            Assert.IsNotNull(created_sub_with_blog);
            Assert.That(created_sub_with_blog.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub_with_blog.Blog.Id, Is.EqualTo(blog.Id));
            Assert.That(created_sub_with_blog.FirstSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(created_sub_with_blog.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Blog.Url, Is.EqualTo(dto.BlogUrl));
            Assert.IsNull(result.GroupId);
        }

        [Test]
        public async Task Subscribe_EnableExistingSubscription_SubscriptionResponseDto()
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

            var blog = new Blog()
            {
                Id = 0,
                Url = dto.BlogUrl
            };

            var sub = new Subscription(user.Id, blog)
            {
                Active = false
            };

            _context.Add(user);
            _context.Add(blog);
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
            var created_sub_with_blog = _context
                .Subscriptions
                .Include(x => x.Blog)
                .Include(x => x.User)
                .Where(x => x.User.Id == user.Id && x.Blog.Url == dto.BlogUrl)
                .First();
            Assert.IsNotNull(created_sub_with_blog);
            Assert.That(created_sub_with_blog.User.Id, Is.EqualTo(user.Id));
            Assert.That(created_sub_with_blog.Blog.Id, Is.EqualTo(blog.Id));
            Assert.That(created_sub_with_blog.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            Assert.That(result.Blog.Url, Is.EqualTo(dto.BlogUrl));
            Assert.IsNull(result.GroupId);
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
