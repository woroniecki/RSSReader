using System.Threading.Tasks;

using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

using ServiceLayer.GroupServices;
using System.Linq;

using DataLayer.Code;
using DbAccess.Core;
using Dtos.Groups;
using DataLayer.Models;

using Tests.Helpers;

namespace Tests.Services.GroupServices
{
    [TestFixture]
    class GroupAddServiceTest
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
        public async Task AddNewGroup_HappyPath_CreatedGroupResponseDto()
        {
            //ARRANGE
            var dto = new AddGroupRequestDto()
            {
                Name = "group"
            };

            var user = new ApiUser()
            {
                Id = "0",
                UserName = "usernaem"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var service = new GroupAddService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.AddNewGroup(dto, user.Id);

            //ASSERT
            Assert.That(_context.Groups.Count(), Is.EqualTo(1));
            var created_group = _context
                .Groups
                .Where(x => x.User.Id == user.Id && x.Id == result.Id)
                .First();
            Assert.That(created_group.Name, Is.EqualTo(dto.Name));
            Assert.That(result.Name, Is.EqualTo(dto.Name));
        }

        [Test]
        public async Task AddNewGroup_GroupWithNameAlreadyExists_FailNull()
        {
            //ARRANGE
            var dto = new AddGroupRequestDto()
            {
                Name = "group"
            };

            var user = new ApiUser()
            {
                Id = "0",
                UserName = "usernaem"
            };

            var group_with_same_name = new Group()
            {
                Id = 0,
                Name = dto.Name,
                User = user
            };

            _context.Users.Add(user);
            _context.Groups.Add(group_with_same_name);
            _context.SaveChanges();

            var service = new GroupAddService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.AddNewGroup(dto, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddNewGroup_CantFindUser_Null()
        {
            //ARRANGE
            var dto = new AddGroupRequestDto()
            {
                Name = "group"
            };

            var service = new GroupAddService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.AddNewGroup(dto, "0");

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

    }
}
