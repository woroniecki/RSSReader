using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Identity;
using RSSReader.Controllers;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using RSSReader.UnitTests.Wrappers;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Dtos;
using System;

using RSSReader.UnitTests.Helpers;

namespace RSSReader.UnitTests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private Mock<IConfiguration> _configurationMock;
        private Dtos.UserForRegisterDto _registerModel;
        private Dtos.UserForLoginDto _loginUsernameModel;
        private UserForLoginDto _loginEmailModel;
        private AuthController _authController;
        private IdentityUser _userToLogin;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
                );

            _configurationMock = new Mock<IConfiguration>();

            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns("you cant break me, secret key");

            _configurationMock.Setup(x => x.GetSection("AppSettings:Token"))
                              .Returns(configSectionMock.Object);

            //Dto
            _registerModel = new Dtos.UserForRegisterDto()
            {
                Username = "username",
                Email = "user@mail.com",
                Password = "password"
            };

            _loginUsernameModel = new Dtos.UserForLoginDto()
            {
                Username = "username",
                Password = "password"
            };

            _loginEmailModel = new Dtos.UserForLoginDto()
            {
                Username = "user@mail.com",
                Password = "password"
            };

            //Controller
            _authController = new AuthController(_userManagerMock.Object, _configurationMock.Object);

            //Data
            _userToLogin = new IdentityUser()
            {
                Id = "0",
                UserName = "username",
                Email = "user@mail.com"

            };
        }

        #region Register

        [Test]
        public async Task Register_CreateNewUser_ReturnCreatedResult()
        {
            //ARRANGE
            var identityResultMock = new Mock<IdentityResultWrapper>(true);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), _registerModel.Password))
                           .Returns(Task.FromResult<IdentityResult>(identityResultMock.Object));

            //ACT
            var result = await _authController.Register(_registerModel);
            var result_data_user = (result as CreatedResult).Value as IdentityUser;

            //ASSERT
            Assert.IsInstanceOf<CreatedResult>(result);
            Assert.That(result_data_user.UserName, Is.EqualTo(_registerModel.Username));
            Assert.That(result_data_user.Email, Is.EqualTo(_registerModel.Email));
        }

        [Test]
        public async Task Register_UsernameAlreadyTaken_ReturnBadRequest()
        {
            //ARRANGE
            _userManagerMock.Setup(x => x.FindByNameAsync(_registerModel.Username))
                            .Returns(Task.FromResult(new IdentityUser()));

            //ACT
            var result = await _authController.Register(_registerModel);

            //ASSERT
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Register_EmailAlreadyTaken_ReturnBadRequest()
        {
            //ARRANGE
            _userManagerMock.Setup(x => x.FindByEmailAsync(_registerModel.Email))
                            .Returns(Task.FromResult(new IdentityUser()));

            //ACT
            var result = await _authController.Register(_registerModel);

            //ASSERT
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Register_CreateUserFails_ReturnBadRequest()
        {
            //ARRANGE
            var identityResultMock = new Mock<IdentityResultWrapper>(false);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), _registerModel.Password))
                           .Returns(Task.FromResult<IdentityResult>(identityResultMock.Object));

            //ACT
            var result = await _authController.Register(_registerModel);

            //ASSERT
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        #endregion

        #region Login

        [Test]
        public async Task Login_LoginByUsername_ReturnOkLoggedUser()
        {
            //ARRANGE
            _userToLogin.UserName = _loginUsernameModel.Username;

            _userManagerMock.Setup(x => x.FindByNameAsync(_loginUsernameModel.Username))
                            .Returns(Task.FromResult(_userToLogin));

            _userManagerMock.Setup(x => x.CheckPasswordAsync(_userToLogin, _loginUsernameModel.Password))
                            .Returns(Task.FromResult(true));

            //ACT
            var result = await _authController.Login(_loginUsernameModel);
            GetDataFromLoginResult(result, out var result_token, out var result_expires, out var result_user);

            //ASSERT
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<string>(result_token);
            Assert.IsTrue(result_expires > DateTime.Now);
            Assert.That(
                result_user.UserName,
                Is.EqualTo(_loginUsernameModel.Username)
                );
        }

        [Test]
        public async Task Login_LoginByEmail_ReturnOkLoggedUser()
        {
            //ARRANGE
            _userToLogin.Email = _loginEmailModel.Username;

            _userManagerMock.Setup(x => x.FindByEmailAsync(_loginEmailModel.Username))
                            .Returns(Task.FromResult(_userToLogin));

            _userManagerMock.Setup(x => x.CheckPasswordAsync(_userToLogin, _loginUsernameModel.Password))
                            .Returns(Task.FromResult(true));

            //ACT
            var result = await _authController.Login(_loginEmailModel);
            GetDataFromLoginResult(result, out var result_token, out var result_expires, out var result_user);

            //ASSERT
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<string>(result_token);
            Assert.IsTrue(result_expires > DateTime.Now);
            Assert.That( result_user.Email, Is.EqualTo(_loginEmailModel.Username));
        }

        [Test]
        public async Task Login_UserWithUsernameOrEmailNotExists_ReturnUnauthorized()
        {
            //ARRANGE
            _userToLogin.UserName = _loginUsernameModel.Username;
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                            .Returns(Task.FromResult<IdentityUser>(null));

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                            .Returns(Task.FromResult<IdentityUser>(null));

            //ACT
            var result = await _authController.Login(_loginUsernameModel);
            var result_data = (result as ObjectResult).Value as string;

            //PASSWORD
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            Assert.That(result_data, Is.EqualTo("Wrong data"));
        }

        [Test]
        public async Task Login_WrongPassword_ReturnUnauthorized()
        {
            //ARRANGE
            _userToLogin.UserName = _loginUsernameModel.Username;
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                            .Returns(Task.FromResult<IdentityUser>(_userToLogin));

            _userManagerMock.Setup(x => x.CheckPasswordAsync(_userToLogin, _loginUsernameModel.Password))
                            .Returns(Task.FromResult(false));

            //ACT
            var result = await _authController.Login(_loginUsernameModel);
            var result_data = (result as ObjectResult).Value as string;

            //PASSWORD
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            Assert.That(result_data, Is.EqualTo("Wrong data"));
        }

        private static void GetDataFromLoginResult(IActionResult result, out string result_token, out DateTime result_expires, out IdentityUser result_user)
        {
            var result_data = (result as ObjectResult).Value;
            result_token = result_data.GetProperty("token") as string;
            result_expires = (DateTime)result_data.GetProperty("expiration");
            result_user = result_data.GetProperty("user") as IdentityUser;
        }

        #endregion
    }
}