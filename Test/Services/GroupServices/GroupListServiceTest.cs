using System;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;

using DataLayer.Code;
using DbAccess.Core;
using DataLayer.Models;
using ServiceLayer.GroupServices;

using Tests.Helpers;
using System.Linq;

namespace Tests.Services.GroupServices
{
    [TestFixture]
    class GroupListServiceTest
    {
        private UnitOfWork _unitOfWork;

        [SetUp]
        public void SetUp()
        {
            var options = InMemoryDb.CreateNewContextOptions();
            var context = new DataContext(options);
            _unitOfWork = new UnitOfWork(context);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Dispose();
        }

        [Test]
        public async Task GetList_HappyPath_ListOfGroups()
        {
            //ARRANGE
            var user1 = new ApiUser() { UserName = "user1" };
            var user2 = new ApiUser() { UserName = "user2" };
            var group1_user1 = new Group() { User = user1 };
            var group2_user1 = new Group() { User = user1 };
            var group3_user2 = new Group() { User = user2 };
            _unitOfWork.Context.Users.Add(user1);
            _unitOfWork.Context.Users.Add(user2);
            _unitOfWork.Context.Groups.Add(group1_user1);
            _unitOfWork.Context.Groups.Add(group2_user1);
            _unitOfWork.Context.Groups.Add(group3_user2);
            _unitOfWork.Context.SaveChanges();

            var service = new GroupListService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.GetList(user1.Id);

            //ASSERT
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.NotNull(result.Where(x => x.Id == group1_user1.Id).First());
            Assert.NotNull(result.Where(x => x.Id == group2_user1.Id).First());
        }

        [Test]
        public async Task GetList_CantFindUserWithProvidedId_Null()
        {
            //ARRANGE
            var service = new GroupListService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.GetList("0");

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }
    }
}
