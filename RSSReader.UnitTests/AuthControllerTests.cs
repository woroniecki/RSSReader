using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Identity;
using RSSReader.Controllers;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using RSSReader.UnitTests.Wrappers;
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
using RSSReader.Data;
using RSSReader.UnitTests.Wrappers.Repositories;

namespace RSSReader.UnitTests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private const string REFRESH_TOKEN_STR = "VJAaEs3Jmk5WgY4/ko/GgeebWEdzMx4peS90Iw5zH34Wfmws5oOTFkHjW4MuXYFo5k45/zA484DLHP7z8XO2mQ==";
        private const string AUTH_TOKEN_STR = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwIiwidW5pcXVlX25hbWUiOiJ1c2VybmFtZSIsIm5iZiI6MTYxMDU2OTUyMCwiZXhwIjoxNjEwNTgwMzIwLCJpYXQiOjE2MTA1Njk1MjB9.ZaFHLEUtyOHqBMgwO5hyR_eWeNvxBGt4ioGplfyHwSeO6gm80f_21laU4bFnDulwUrAgrc25GFvOSeVpE9_nBw";

        private Mock<UserManager<ApiUser>> _userManagerMock;
        private IAuthService _authservice;
        private IMapper _mapper;
        private MockUOW _mockUOW;
        private Dtos.UserForRegisterDto _registerModel;
        private Dtos.UserForLoginDto _loginUsernameModel;
        private UserForLoginDto _loginEmailModel;
        private AuthController _authController;
        private ApiUser _userToLogin;
        private DataForRefreshTokenDto _dataForRefreshTokenDto;
        private Mock<Data.Repositories.IReaderRepository> _readerRepo;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _userManagerMock = new Mock<UserManager<ApiUser>>(
                Mock.Of<IUserStore<ApiUser>>(),
                null, null, null, null, null, null, null, null
                );

            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns("you cant break me, secret key");
            Mock<IConfiguration> configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x.GetSection("AppSettings:Token"))
                .Returns(configSectionMock.Object);

            _mockUOW = new MockUOW();

            _authservice = new AuthService(configuration.Object, _mockUOW.Object);

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

            //Data
            _userToLogin = new ApiUser()
            {
                Id = "0",
                UserName = "username",
                Email = "user@mail.com",
                RefreshTokens = new List<RefreshToken>()
            };

            _dataForRefreshTokenDto = new DataForRefreshTokenDto()
            {
                RefreshToken = REFRESH_TOKEN_STR,
                AuthToken = AUTH_TOKEN_STR
            };

            //Controller
            _authController = new AuthController(
                _userManagerMock.Object, _mockUOW.Object, _authservice, _mapper
                );
        }

        #region Mocks

        private void Mock_ReaderRepository_SaveAllAsync(bool returnedValue)
        {
            _readerRepo.Setup(x => x.SaveAllAsync())
                            .Returns(Task.FromResult(returnedValue));
        }

        private void Mock_UserManager_CreateAsync(string password, bool identityResultValue = true)
        {
            var identityResultMock = new Mock<IdentityResultWrapper>(identityResultValue);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApiUser>(), password))
                            .Returns(Task.FromResult<IdentityResult>(identityResultMock.Object))
                            .Verifiable();
        }

        private void Mock_UserManager_CheckPasswordAsync(ApiUser user, string password, bool returnedValue)
        {
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password))
                                        .Returns(Task.FromResult(returnedValue));
        }

        #endregion

        #region Register
        [Test]
        public async Task Register_CreateNewUser_CreatedResponse()
        {
            //ARRANGE
            Mock_UserManager_CreateAsync(_registerModel.Password);

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
            _mockUOW.UserRepo.SetGetByUsername(
                _registerModel.Username,
                new ApiUser() { UserName = _registerModel.Username }
                );

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
            _mockUOW.UserRepo.SetGetByEmail(
                _registerModel.Email,
                new ApiUser() { Email = _registerModel.Email }
                );

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
        public async Task Register_CreateUserFails_RequestFailed()
        {
            //ARRANGE
            Mock_UserManager_CreateAsync(_registerModel.Password, false);

            //ACT
            var result = await _authController.Register(_registerModel);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrRequestFailed));
        }

        #endregion

        #region Login

        [Test]
        public async Task Login_LoginByUsername_OkLoggedUser()
        {
            //ARRANGE
            _userToLogin.UserName = _loginUsernameModel.Username;

            _mockUOW.UserRepo.SetGetWithRefreshTokens(_userToLogin);
            Mock_UserManager_CheckPasswordAsync(
                _userToLogin, _loginUsernameModel.Password, true
                );
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            //ACT
            var result = await _authController.Login(_loginUsernameModel);
            GetDataFromLoginResult(result, out var result_token, out var result_user, out var refresh_token);

            //ASSERT
            AssertTokensResult(result, _loginUsernameModel.Username);
        }

        [Test]
        public async Task Login_LoginByEmail_OkLoggedUser()
        {
            //ARRANGE
            _userToLogin.Email = _loginEmailModel.Username;
            _mockUOW.UserRepo.SetGetWithRefreshTokens(_userToLogin);
            Mock_UserManager_CheckPasswordAsync(
                _userToLogin, _loginUsernameModel.Password, true
                );
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            //ACT
            var result = await _authController.Login(_loginEmailModel);
            GetDataFromLoginResult(result, out var result_token, out var result_user, out var refresh_token);

            //ASSERT
            //Is logging by email so verify email
            Assert.That(result_user.Email, Is.EqualTo(_loginEmailModel.Username));
            AssertTokensResult(result, _userToLogin.UserName);
        }

        [Test]
        public async Task Login_UserWithUsernameOrEmailNotExists_ErrWrongCredentials()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithRefreshTokens(null);

            //ACT
            var result = await _authController.Login(_loginUsernameModel);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrWrongCredentials));
        }

        [Test]
        public async Task Login_WrongPassword_ErrWrongCredentials()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithRefreshTokens(_userToLogin);
            Mock_UserManager_CheckPasswordAsync(
                _userToLogin, _loginUsernameModel.Password, false
                );

            //ACT
            var result = await _authController.Login(_loginUsernameModel);
            var result_data = result.Result as string;

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrWrongCredentials));
        }

        private static void GetDataFromLoginResult(ApiResponse result, out TokenForReturnDto auth_token, out UserForReturnDto result_user, out TokenForReturnDto refresh_token)
        {
            var result_data = result.Result;
            auth_token = result_data.GetProperty("authToken") as TokenForReturnDto;
            refresh_token = result_data.GetProperty("refreshToken") as TokenForReturnDto;
            result_user = result_data.GetProperty("user") as UserForReturnDto;
        }

        #endregion

        #region Refresh

        [Test]
        public async Task Refresh_CantGetUserIdFromClaims_ErrUnauthorized()
        {
            //ARRANGE
            
            //ACT
            var result = await _authController.Refresh(_dataForRefreshTokenDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Refresh_CantFindUserWithId_ErrUnauhtorized()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithRefreshTokens(null);

            //ACT
            var result = await _authController.Refresh(_dataForRefreshTokenDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));

        }

        [Test]
        public async Task Refresh_CantFindRefreshToken_ErrEntityNotExists()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithRefreshTokens(_userToLogin);

            _userToLogin.RefreshTokens = Enumerable.Repeat(
                new RefreshToken() { Token = "Wrong" }, 3)
                .ToList();

            //ACT
            var result = await _authController.Refresh(_dataForRefreshTokenDto);

            //ASSERT
            Assert.That(result.Message, Is.EqualTo(ErrEntityNotExists.Message));
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        [Test]
        public async Task Refresh_PrivdedAuthTokenOtherThanInRefresh_Unauthorized()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithRefreshTokens(_userToLogin);

            RefreshToken old_token = new RefreshToken()
            {
                Token = REFRESH_TOKEN_STR,
                AuthToken = AUTH_TOKEN_STR + "WRONG",
                Expires = DateTime.UtcNow.AddMinutes(10)
            };
            _userToLogin.RefreshTokens = Enumerable.Repeat(old_token, 1).ToList();

            //ACT
            var result = await _authController.Refresh(_dataForRefreshTokenDto);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status401Unauthorized));
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Refresh_RefreshTokenAlreadyInactive_ErrBadRequest()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithRefreshTokens(_userToLogin);

            RefreshToken old_token = new RefreshToken()
            {
                Token = REFRESH_TOKEN_STR,
                AuthToken = AUTH_TOKEN_STR,
                Expires = DateTime.UtcNow.AddMinutes(-1)
            };
            _userToLogin.RefreshTokens = Enumerable.Repeat(old_token, 1).ToList();

            //ACT
            var result = await _authController.Refresh(_dataForRefreshTokenDto);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status400BadRequest));
            Assert.That(result.Message, Is.EqualTo(MsgErrBadRequest));
        }

        [Test]
        public async Task Refresh_RefreshTokens_OkAuthTokenAndRefreshToken()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithRefreshTokens(_userToLogin);

            RefreshToken token = new RefreshToken()
            {
                Token = REFRESH_TOKEN_STR,
                AuthToken = AUTH_TOKEN_STR,
                Expires = DateTime.UtcNow.AddMinutes(10)
            };
            _userToLogin.RefreshTokens = Enumerable.Repeat(token, 1).ToList();
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            //ACT
            var result = await _authController.Refresh(_dataForRefreshTokenDto);

            //ASSERT
            Assert.That(token.Revoked, Is.LessThanOrEqualTo(DateTime.UtcNow));
            Assert.False(token.IsActive);
            AssertTokensResult(result, _userToLogin.UserName);
        }

        #endregion

        #region ASSERTIONS

        private static void AssertTokensResult(ApiResponse result, string providedUsername)
        {
            GetDataFromLoginResult(result, out var result_token, out var result_user, out var refresh_token);
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.IsInstanceOf<string>(result_token.Token);
            Assert.That(result_token.Expires, Is.GreaterThan(DateTime.UtcNow.From1970()));
            Assert.IsInstanceOf<string>(refresh_token.Token);
            Assert.That(refresh_token.Expires, Is.GreaterThan(DateTime.UtcNow.From1970()));
            Assert.That(
                result_user.UserName,
                Is.EqualTo(providedUsername)
                );
        }

        #endregion
    }
}