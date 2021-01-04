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
using RSSReader.Models;
using AutoMapper;
using RSSReader.Helpers;
using AutoWrapper.Wrappers;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using System.Linq;

namespace RSSReader.UnitTests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<UserManager<ApiUser>> _userManagerMock;
        private Mock<IConfiguration> _configurationMock;
        private IMapper _mapper;
        private Dtos.UserForRegisterDto _registerModel;
        private Dtos.UserForLoginDto _loginUsernameModel;
        private UserForLoginDto _loginEmailModel;
        private AuthController _authController;
        private ApiUser _userToLogin;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _userManagerMock = new Mock<UserManager<ApiUser>>(
                Mock.Of<IUserStore<ApiUser>>(),
                null, null, null, null, null, null, null, null
                );

            _configurationMock = new Mock<IConfiguration>();

            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns("you cant break me, secret key");

            _configurationMock.Setup(x => x.GetSection("AppSettings:Token"))
                              .Returns(configSectionMock.Object);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });
            _mapper = mockMapper.CreateMapper();

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
            _authController = new AuthController(_userManagerMock.Object, _configurationMock.Object, _mapper);

            //Data
            _userToLogin = new ApiUser()
            {
                Id = "0",
                UserName = "username",
                Email = "user@mail.com"
            };
        }

        #region Register

        [Test]
        public async Task Register_CreateNewUser_CreatedResponse()
        {
            //ARRANGE
            var identityResultMock = new Mock<IdentityResultWrapper>(true);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApiUser>(), _registerModel.Password))
                           .Returns(Task.FromResult<IdentityResult>(identityResultMock.Object));

            //ACT
            var result = await _authController.Register(_registerModel);
            var result_data_user = result.Result as UserForReturnDto;

            //ASSERT
            Assert.That(result.Message, Is.EqualTo(MsgCreatedRecord));
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
            Assert.That(result_data_user.UserName, Is.EqualTo(_registerModel.Username));
            Assert.That(result_data_user.Email, Is.EqualTo(_registerModel.Email));
        }

        [Test]
        public async Task Register_UsernameAlreadyTaken_UsernameFieldError()
        {
            //ARRANGE
            _userManagerMock.Setup(x => x.FindByNameAsync(_registerModel.Username))
                            .Returns(Task.FromResult(new ApiUser()));

            //ACT
            var ex = Assert.ThrowsAsync<ApiProblemDetailsException>(
                () => _authController.Register(_registerModel)
                );

            var error_key = _authController.ModelState.Keys.First();
            var error_msg = _authController.ModelState[error_key]
                .Errors.First().ErrorMessage;

            //ASSERT
            Assert.That(ex.StatusCode, Is.EqualTo(Status422UnprocessableEntity));
            Assert.False(_authController.ModelState.IsValid);
            Assert.That(error_key, Is.EqualTo(nameof(UserForRegisterDto.Username)));
            Assert.That(error_msg, Is.EqualTo(MsgErrUsernameTaken));
        }

        [Test]
        public async Task Register_EmailAlreadyTaken_EmailFieldError()
        {
            //ARRANGE
            _userManagerMock.Setup(x => x.FindByEmailAsync(_registerModel.Email))
                            .Returns(Task.FromResult(new ApiUser()));

            //ACT
            var ex = Assert.ThrowsAsync<ApiProblemDetailsException>(
                () => _authController.Register(_registerModel)
                );

            var error_key = _authController.ModelState.Keys.First();
            var error_msg = _authController.ModelState[error_key]
                .Errors.First().ErrorMessage;

            //ASSERT
            Assert.That(ex.StatusCode, Is.EqualTo(Status422UnprocessableEntity));
            Assert.False(_authController.ModelState.IsValid);
            Assert.That(error_key, Is.EqualTo(nameof(UserForRegisterDto.Email)));
            Assert.That(error_msg, Is.EqualTo(MsgErrEmailTaken));
        }

        [Test]
        public async Task Register_CreateUserFails_BadRequestResponse()
        {
            //ARRANGE
            var identityResultMock = new Mock<IdentityResultWrapper>(false);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApiUser>(), _registerModel.Password))
                           .Returns(Task.FromResult<IdentityResult>(identityResultMock.Object));

            //ACT
            var result = await _authController.Register(_registerModel);

            //ASSERT
            Assert.That(result.Message, Is.EqualTo(MsgErrRequestFailed));
            Assert.That(result.StatusCode, Is.EqualTo(Status400BadRequest));
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
                            .Returns(Task.FromResult<ApiUser>(null));

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                            .Returns(Task.FromResult<ApiUser>(null));

            //ACT
            var result = await _authController.Login(_loginUsernameModel);
            var result_data = result.Result as string;

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
                            .Returns(Task.FromResult<ApiUser>(_userToLogin));

            _userManagerMock.Setup(x => x.CheckPasswordAsync(_userToLogin, _loginUsernameModel.Password))
                            .Returns(Task.FromResult(false));

            //ACT
            var result = await _authController.Login(_loginUsernameModel);
            var result_data = result.Result as string;

            //PASSWORD
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            Assert.That(result_data, Is.EqualTo("Wrong data"));
        }

        private static void GetDataFromLoginResult(ApiResponse result, out string result_token, out DateTime result_expires, out UserForReturnDto result_user)
        {
            var result_data = result.Result;
            result_token = result_data.GetProperty("token") as string;
            result_expires = (DateTime)result_data.GetProperty("expiration");
            result_user = result_data.GetProperty("user") as UserForReturnDto;
        }

        #endregion
    }
}