using System;
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
    class SubscriptionSetGroupServiceTest
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
        public async Task SetGroup_SetGroup_EditedSubscription()
        {
            //ARRANGE
            var user = new ApiUser();
            _context.Add(user);

            var blog = new Blog();
            _context.Add(blog);

            var group_to_set = new Group() { User = user };
            _context.Add(group_to_set);

            var sub = new Subscription() { UserId = user.Id, Group = null, Blog = blog };
            _context.Add(sub);

            _context.SaveChanges();

            var service = new SubscriptionSetGroupService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT
            var result = await service.SetGroup(sub.Id, group_to_set.Id, user.Id);

            //ASSERT
            Assert.That(result.Id, Is.EqualTo(blog.Id));
            Assert.That(result.UserData.GroupId, Is.EqualTo(group_to_set.Id));
            var edited_sub = _context.Subscriptions.First(x => x.Id == sub.Id);
            Assert.That(edited_sub.GroupId, Is.EqualTo(group_to_set.Id));
        }

        [Test]
        public async Task SetGroup_SetGroupToNone_EditedSubscription()
        {
            //ARRANGE
            var user = new ApiUser();
            _context.Add(user);

            var blog = new Blog();
            _context.Add(blog);

            var group_to_set = new Group() { User = user };
            _context.Add(group_to_set);

            var sub = new Subscription() { UserId = user.Id, GroupId = group_to_set.Id, Blog = blog };
            _context.Add(sub);

            _context.SaveChanges();

            var service = new SubscriptionSetGroupService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.SetGroup(sub.Id, -1, user.Id);

            //ASSERT
            Assert.That(result.Id, Is.EqualTo(blog.Id));
            Assert.IsNull(result.UserData.GroupId);
            var edited_sub = _context.Subscriptions.Include(x => x.Group).First(x => x.Id == sub.Id);
            Assert.IsNull(edited_sub.GroupId);
        }

        [Test]
        public async Task SetGroup_SubWithIdDoesntExist_Null()
        {
            //ARRANGE
            var user = new ApiUser();
            _context.Add(user);

            var group_to_set = new Group() { User = user };
            _context.Add(group_to_set);

            _context.SaveChanges();

            var service = new SubscriptionSetGroupService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT
            var result = await service.SetGroup(0, group_to_set.Id, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task SetGroup_GroupWithIdDoesntExist_Null()
        {
            //ARRANGE
            var user = new ApiUser();
            _context.Add(user);

            var sub = new Subscription() { UserId = user.Id, Blog = new Blog() };
            _context.Add(sub);


            _context.SaveChanges();

            var service = new SubscriptionSetGroupService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT
            var result = await service.SetGroup(sub.Id, 0, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task SetGroup_SubBelongsToOtherUser_Null()
        {
            //ARRANGE
            var user = new ApiUser();
            var user2 = new ApiUser();
            _context.Add(user);
            _context.Add(user2);

            var group_to_set = new Group() { User = user };
            _context.Add(group_to_set);

            var sub = new Subscription() { UserId = user2.Id, Blog = new Blog() };
            _context.Add(sub);

            _context.SaveChanges();

            var service = new SubscriptionSetGroupService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT
            var result = await service.SetGroup(sub.Id, group_to_set.Id, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task SetGroup_GroupBelongsToOtherUser_Null()
        {
            //ARRANGE
            var user = new ApiUser();
            var user2 = new ApiUser();
            _context.Add(user);
            _context.Add(user2);

            var group_to_set = new Group() { User = user2 };
            _context.Add(group_to_set);

            var sub = new Subscription() { UserId = user.Id, Blog = new Blog() };
            _context.Add(sub);

            _context.SaveChanges();

            var service = new SubscriptionSetGroupService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT
            var result = await service.SetGroup(sub.Id, group_to_set.Id, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }
    }
}
