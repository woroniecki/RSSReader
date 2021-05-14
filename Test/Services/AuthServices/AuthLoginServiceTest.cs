using System;
using System.Threading.Tasks;
using System.Linq;

using NUnit.Framework;
using Moq;

using DataLayer.Code;
using DbAccess.Core;
using ServiceLayer.AuthServices;
using Dtos.Auth.Login;
using DataLayer.Models;
using LogicLayer.Helpers;

using Tests.Helpers;
using Tests.Fake;

namespace Tests.Services.AuthServices
{
    [TestFixture]
    class AuthLoginServiceTest
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
        public async Task Login_NotExistingUserWithProvidedCredentials_Null()
        {
            //ARRANGE
            var dto = new LoginRequestDto()
            {
                Username = "username",
                Password = "password"
            };

            var service = new AuthLoginService(
                new FakeUserManager().Object,
                MapperHelper.GetNewInstance(),
                null,
                _unitOfWork
                );

            //ACT
            var result = await service.Login(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Login_WrongPassword_Null()
        {
            //ARRANGE
            string USERNAME_VALUE = "username";
            var dto = new LoginRequestDto()
            {
                Username = USERNAME_VALUE,
                Password = "password"
            };

            var usermanager = new FakeUserManager();
            usermanager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApiUser>(), dto.Password))
                .Returns(Task.FromResult(false))
                .Verifiable();

            var service = new AuthLoginService(
                usermanager.Object,
                MapperHelper.GetNewInstance(),
                null,
                _unitOfWork
                );

            _unitOfWork.Context.Users.Add(new ApiUser()
            {
                UserName = USERNAME_VALUE
            });
            _unitOfWork.Context.SaveChanges();

            //ACT
            var result = await service.Login(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            usermanager.Verify();
        }

        [Test]
        public async Task Login_SuccessByUsername_TokensAndUser()
        {
            //ARRANGE
            var config = new FakeConfigGetToken();

            string USERNAME_VALUE = "username";
            var dto = new LoginRequestDto()
            {
                Username = USERNAME_VALUE,
                Password = "password"
            };

            var usermanager = new FakeUserManager();
            usermanager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApiUser>(), dto.Password))
                .Returns(Task.FromResult(true))
                .Verifiable();

            var service = new AuthLoginService(
                usermanager.Object,
                MapperHelper.GetNewInstance(),
                config.Object,
                _unitOfWork
                );

            _unitOfWork.Context.Users.Add(new ApiUser()
            {
                UserName = USERNAME_VALUE
            });
            _unitOfWork.Context.SaveChanges();

            //ACT
            var result = await service.Login(dto);

            //ASSERT
            Assert.That(result.User.UserName, Is.EqualTo(dto.Username));
            Assert.Greater(result.AuthToken.Token.Length, 0);
            Assert.Greater(result.AuthToken.Expires, DateTime.UtcNow.From1970());
            Assert.Greater(result.RefreshToken.Token.Length, 0);
            Assert.Greater(result.RefreshToken.Expires, DateTime.UtcNow.From1970());
            var user_ref_tokens = _unitOfWork.Context.Users.First().RefreshTokens;
            Assert.That(user_ref_tokens.Count, Is.EqualTo(1));
            Assert.Zero(service.Errors.Count);
            usermanager.Verify();
            config.Verify();
        }

        [Test]
        public async Task Login_SuccessByEmail_TokensAndUser()
        {
            //ARRANGE
            var config = new FakeConfigGetToken();

            string EMAIL_VALUE = "email";
            var dto = new LoginRequestDto()
            {
                Username = EMAIL_VALUE,
                Password = "password"
            };

            var usermanager = new FakeUserManager();
            usermanager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApiUser>(), dto.Password))
                .Returns(Task.FromResult(true))
                .Verifiable();

            var service = new AuthLoginService(
                usermanager.Object,
                MapperHelper.GetNewInstance(),
                config.Object,
                _unitOfWork
                );

            _unitOfWork.Context.Users.Add(new ApiUser()
            {
                UserName = "username",
                Email = EMAIL_VALUE
            });
            _unitOfWork.Context.SaveChanges();

            //ACT
            var result = await service.Login(dto);

            //ASSERT
            Assert.That(result.User.Email, Is.EqualTo(dto.Username));
            Assert.Greater(result.AuthToken.Token.Length, 0);
            Assert.Greater(result.AuthToken.Expires, DateTime.UtcNow.From1970());
            Assert.Greater(result.RefreshToken.Token.Length, 0);
            Assert.Greater(result.RefreshToken.Expires, DateTime.UtcNow.From1970());
            var user_ref_tokens = _unitOfWork.Context.Users.First().RefreshTokens;
            Assert.That(user_ref_tokens.Count, Is.EqualTo(1));
            Assert.Zero(service.Errors.Count);
            usermanager.Verify();
            config.Verify();
        }
    }
}
