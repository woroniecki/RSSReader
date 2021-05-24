using System.Threading.Tasks;

using NUnit.Framework;

using DataLayer.Code;
using DbAccess.Core;

using Tests.Helpers;
using ServiceLayer.GroupServices;
using DataLayer.Models;
using System.Linq;
using Dtos.Groups;
using Microsoft.EntityFrameworkCore;

namespace Tests.Services.GroupServices
{
    [TestFixture]
    class GroupRemoveServiceTest
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
        public async Task Remove_HappyPath_Success()
        {
            //ARRANGE
            var user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            var group_to_remove = new Group()
            {
                Id = 0,
                Name = "name",
                User = user
            };

            _context.Users.Add(user);
            _context.Groups.Add(group_to_remove);
            _context.SaveChanges();

            var dto = new RemoveGroupRequestDto() { GroupId = group_to_remove.Id };

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(dto, user.Id);

            //ASSERT
            Assert.Zero(_context.Groups.Where(x => x.Id == group_to_remove.Id).Count());
            Assert.That(service.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Remove_UserNotExits_Fail()
        {
            //ARRANGE
            var user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            var group_to_remove = new Group()
            {
                Id = 0,
                Name = "name",
                User = user
            };

            _context.Users.Add(user);
            _context.Groups.Add(group_to_remove);
            _context.SaveChanges();

            var dto = new RemoveGroupRequestDto() { GroupId = group_to_remove.Id };

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(dto, "wrongid");

            //ASSERT
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Remove_GroupNotExists_Fail()
        {
            //ARRANGE
            var user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var dto = new RemoveGroupRequestDto() { GroupId = 0 };

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(dto, user.Id);

            //ASSERT
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Remove_GroupNotBelongsToUser_Fail()
        {
            //ARRANGE
            var user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            var other_user = new ApiUser()
            {
                Id = "1",
                UserName = "username2"
            };

            var group_to_remove = new Group()
            {
                Id = 0,
                Name = "name",
                User = other_user
            };

            _context.Users.Add(user);
            _context.Users.Add(other_user);
            _context.Groups.Add(group_to_remove);
            _context.SaveChanges();

            var dto = new RemoveGroupRequestDto() { GroupId = group_to_remove.Id };

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(dto, user.Id);

            //ASSERT
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Remove_ResetAllSubscriptionsGroup_Success()
        {
            //ARRANGE
            var user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            var group_to_remove = new Group()
            {
                Id = 0,
                Name = "name",
                User = user
            };

            _context.Groups.Add(group_to_remove);

            var sub1 = new Subscription()
            {
                Group = group_to_remove,
                GroupId = group_to_remove.Id,
                UserId = user.Id
            };

            var sub2 = new Subscription()
            {
                Group = group_to_remove,
                GroupId = group_to_remove.Id,
                UserId = user.Id
            };

            var sub3 = new Subscription();
            _context.Add(sub1);
            _context.Add(sub2);
            _context.Add(sub3);

            _context.Users.Add(user);
            _context.SaveChanges();

            var dto = new RemoveGroupRequestDto() { GroupId = group_to_remove.Id };

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(dto, user.Id);

            //ASSERT
            Assert.Zero(service.Errors.Count);
            Assert.Zero(_context.Groups.Where(x => x.Id == group_to_remove.Id).Count());
            var subs = _context.Subscriptions.Include(x => x.Group).ToList();
            foreach (var sub in subs)
                Assert.IsNull(sub.GroupId);
            Assert.That(service.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Remove_UnsubscribeAllSubscribtions_Success()
        {
            //ARRANGE
            var user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            var group_to_remove = new Group()
            {
                Id = 0,
                Name = "name",
                User = user
            };

            _context.Groups.Add(group_to_remove);

            var sub1 = new Subscription()
            {
                Group = group_to_remove,
                GroupId = group_to_remove.Id,
                Active = true,
                UserId = user.Id
            };

            var sub2 = new Subscription()
            {
                Group = group_to_remove,
                GroupId = group_to_remove.Id,
                Active = false,
                UserId = user.Id
            };

            var sub3 = new Subscription() { Active = true };
            _context.Add(sub1);
            _context.Add(sub2);
            _context.Add(sub3);

            _context.Users.Add(user);
            _context.SaveChanges();

            var dto = new RemoveGroupRequestDto() { 
                GroupId = group_to_remove.Id, 
                UnsubscribeSubscriptions = true 
            };

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(dto, user.Id);

            //ASSERT
            Assert.Zero(service.Errors.Count, "Has errors");
            Assert.Zero(_context.Groups.Where(x => x.Id == group_to_remove.Id).Count());
            var subs = _context.Subscriptions.ToList();
            foreach (var sub in subs)
                Assert.IsNull(sub.GroupId);
            Assert.That(
                _context.Subscriptions.Where(x => !x.Active).ToList().Count(), 
                Is.EqualTo(2)
                );//As one sub was added without any group, the rest were removed
            Assert.That(service.Errors.Count, Is.EqualTo(0));
        }
    }
}
