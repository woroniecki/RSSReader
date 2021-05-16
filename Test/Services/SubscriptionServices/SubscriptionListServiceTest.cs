using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ServiceLayer.SubscriptionServices;
using Tests.Helpers;

namespace Tests.Services.SubscriptionServices
{
    [TestFixture]
    class SubscriptionListServiceTest
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
        public async Task GetListAsync_GetsSubsAssignedToUser_ListOfActiveSubs()
        {
            //ARRANGE
            var user0 = new ApiUser();
            var user1 = new ApiUser();
            _context.Add(user0);
            _context.Add(user1);

            var group = new Group();
            _context.Add(group);

            var blog0 = new Blog();
            var blog1 = new Blog();
            _context.Add(blog0);
            _context.Add(blog1);

            var sub0_user0 = new Subscription() { Active = true, Blog = blog0, UserId = user0.Id, GroupId = group.Id };
            var sub1_user0 = new Subscription() { Active = false, Blog = blog1, UserId = user0.Id };
            var sub2_user1 = new Subscription() { Active = true, Blog = blog0, UserId = user1.Id };
            var sub3_user1 = new Subscription() { Active = false, Blog = blog1, UserId = user1.Id };
            _context.Add(sub0_user0);
            _context.Add(sub1_user0);
            _context.Add(sub2_user1);
            _context.Add(sub3_user1);

            _context.SaveChanges();

            var service = new SubscriptionListService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.GetListAsync(user0.Id);

            //ASSERT
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(sub0_user0.Id));
            Assert.That(result.First().GroupId, Is.EqualTo(group.Id));
            Assert.That(result.First().Blog.Id, Is.EqualTo(blog0.Id));
        }

        [Test]
        public async Task GetListAsync_GetsSubsAndCountUnreaded_ListOfActiveSubs()
        {
            //ARRANGE
            var user = new ApiUser();
            _context.Add(user);

            var group = new Group();
            _context.Add(group);

            var blog = new Blog();
            var post1 = new Post();
            var post2 = new Post();
            var post3 = new Post();
            blog.Posts = new List<Post>();
            blog.Posts.Add(post1);
            blog.Posts.Add(post2);
            blog.Posts.Add(post3);
            _context.Add(blog);

            var sub = new Subscription() { Active = true, Blog = blog, UserId = user.Id, GroupId = group.Id };
            _context.Add(sub);

            var userpostdata = new UserPostData(post2, user, sub) { Readed = true };
            _context.Add(userpostdata);

            _context.SaveChanges();

            var service = new SubscriptionListService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.GetListAsync(user.Id);

            //ASSERT
            Assert.That(result.First().UnreadedCount, Is.EqualTo(2));
        }
    }
}
