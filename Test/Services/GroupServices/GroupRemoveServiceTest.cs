using System.Threading.Tasks;

using NUnit.Framework;

using DataLayer.Code;
using DbAccess.Core;

using Tests.Helpers;
using ServiceLayer.GroupServices;
using DataLayer.Models;
using System.Linq;

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

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(group_to_remove.Id, user.Id);

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

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(group_to_remove.Id, "wrongid");

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

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(0, user.Id);

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

            var service = new GroupRemoveService(_unitOfWork);

            //ACT
            await service.Remove(group_to_remove.Id, user.Id);

            //ASSERT
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }
    }
}
