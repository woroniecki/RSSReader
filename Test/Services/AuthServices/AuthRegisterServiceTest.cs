using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using Moq;

using DataLayer.Code;
using DbAccess.Core;
using ServiceLayer.AuthServices;
using DataLayer.Models;
using Dtos.Auth.Register;

using Tests.Helpers;
using Tests.Fake;


namespace Tests.Services.AuthServices
{
    [TestFixture]
    class AuthRegisterServiceTest
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
        public async Task RegisterNewUser_CreateNewUser_RegisteredUserDto()
        {
            //ARRANGE
            var dto = new RegisterNewUserRequestDto() { 
                Username = "username", Password = "password"
            };
            var user_manager = new FakeUserManager();
            user_manager.Setup(x => x.CreateAsync(It.IsAny<ApiUser>(), dto.Password))
                        .Returns(Task.FromResult(IdentityResult.Success))
                        .Verifiable();

            var service = new AuthRegisterService(
                user_manager.Object,
                MapperHelper.GetNewInstance(),
                _unitOfWork
            );

            //ACT
            var result = await service.RegisterNewUser(dto);

            //ASSERT
            Assert.That(result.UserName, Is.EqualTo(dto.Username));
            Assert.That(result.Email, Is.EqualTo(dto.Email));
            Assert.Zero(service.Errors.Count);
            user_manager.Verify();
        }

        [Test]
        public async Task RegisterNewUser_UsernameTakenError_Null()
        {
            //ARRANGE
            string USERNAME_VALUE = "username";
            var dto = new RegisterNewUserRequestDto()
            {
                Username = USERNAME_VALUE
            };
            var user_manager = new FakeUserManager();

            var service = new AuthRegisterService(
                user_manager.Object,
                MapperHelper.GetNewInstance(),
                _unitOfWork
            );

            _unitOfWork.Context.Users.Add(new ApiUser()
            {
                UserName = USERNAME_VALUE
            });
            _unitOfWork.Context.SaveChanges();

            //ACT
            var result = await service.RegisterNewUser(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task RegisterNewUser_EmailTakenError_Null()
        {
            //ARRANGE
            string EMAIL_VALUE = "email";
            var dto = new RegisterNewUserRequestDto()
            {
                Username = "user1",
                Email = EMAIL_VALUE
            };
            var user_manager = new FakeUserManager();

            var service = new AuthRegisterService(
                user_manager.Object,
                MapperHelper.GetNewInstance(),
                _unitOfWork
            );

            _unitOfWork.Context.Users.Add(new ApiUser()
            {
                UserName = "user2",
                Email = EMAIL_VALUE
            });
            _unitOfWork.Context.SaveChanges();

            //ACT
            var result = await service.RegisterNewUser(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task RegisterNewUser_UserManagerFailsToCreateUser_Null()
        {
            //ARRANGE
            var dto = new RegisterNewUserRequestDto()
            {
                Username = "username",
                Password = "password"
            };
            var user_manager = new FakeUserManager();
            user_manager.Setup(x => x.CreateAsync(It.IsAny<ApiUser>(), dto.Password))
                        .Returns(Task.FromResult(IdentityResult.Failed()))
                        .Verifiable();

            var service = new AuthRegisterService(
                user_manager.Object,
                MapperHelper.GetNewInstance(),
                _unitOfWork
            );

            //ACT
            var result = await service.RegisterNewUser(dto);

            //ASSERT
            Assert.Null(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            user_manager.Verify();
        }
    }
}
