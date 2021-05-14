using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;
using Moq;

using DataLayer.Code;
using DbAccess.Core;
using ServiceLayer.AuthServices;
using Dtos.Auth.Refresh;
using DataLayer.Models;
using LogicLayer.Helpers;

using Tests.Helpers;
using Tests.Fake;
using LogicLayer.Auth;

namespace Tests.Services.AuthServices
{
    [TestFixture]
    class AuthRefreshTokensServiceTest
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
        public async Task Refresh_HappyPath_NewTokens()
        {
            //ARRANGE
            var config = new FakeConfigGetToken();

            var service = new AuthRefreshTokensService(
                MapperHelper.GetNewInstance(),
                config.Object,
                _unitOfWork
                );

            var new_user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            string auth_key = GenerateAuthTokensAction.CreateAuthToken(
                new_user.Id, new_user.UserName, FakeConfigGetToken.KEY, out DateTime time);
            var exisiting_refresh_token = GenerateAuthTokensAction.CreateRefreshToken(auth_key);

            var tokens = new List<RefreshToken>();
            tokens.Add(exisiting_refresh_token);
            new_user.RefreshTokens = tokens;

            _unitOfWork.Context.Users.Add(new_user);
            _unitOfWork.Context.SaveChanges();

            var dto = new TokensRequestDto()
            {
                AuthToken = auth_key,
                RefreshToken = exisiting_refresh_token.Token
            };

            //ACT
            var result = await service.RefreshTokens(dto);

            //ASSERT
            Assert.That(result.User.UserName, Is.EqualTo(new_user.UserName));
            Assert.Greater(result.AuthToken.Token.Length, 0);
            Assert.Greater(result.AuthToken.Expires, DateTime.UtcNow.From1970());
            Assert.Greater(result.RefreshToken.Token.Length, 0);
            Assert.Greater(result.RefreshToken.Expires, DateTime.UtcNow.From1970());
            var user_ref_tokens = _unitOfWork.Context.Users.First().RefreshTokens;
            Assert.That(user_ref_tokens.Count, Is.EqualTo(2));//One created before act and one created in act
            Assert.IsFalse(
                user_ref_tokens.Where(x => x.Token == exisiting_refresh_token.Token)
                .First().IsActive
                );//One created before act and one created in act
            Assert.Zero(service.Errors.Count);
            config.Verify();
        }

        [Test]
        public async Task Refresh_CantGetIdFromAuthToken_Null()
        {
            //ARRANGE
            var config = new FakeConfigGetToken();

            var service = new AuthRefreshTokensService(
                MapperHelper.GetNewInstance(),
                config.Object,
                _unitOfWork
                );

            var new_user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            string auth_key = GenerateAuthTokensAction.CreateAuthToken(
                new_user.Id, new_user.UserName, "Not exisiting key!!!!!!!!!!!!!!!", out DateTime time);
            var exisiting_refresh_token = GenerateAuthTokensAction.CreateRefreshToken(auth_key);

            var dto = new TokensRequestDto()
            {
                AuthToken = auth_key,
                RefreshToken = exisiting_refresh_token.Token
            };

            //ACT
            var result = await service.RefreshTokens(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            config.Verify();
        }

        [Test]
        public async Task Refresh_CantFindUserWithIdFromToken_Null()
        {
            //ARRANGE
            var config = new FakeConfigGetToken();

            var service = new AuthRefreshTokensService(
                MapperHelper.GetNewInstance(),
                config.Object,
                _unitOfWork
                );

            var new_user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            string auth_key = GenerateAuthTokensAction.CreateAuthToken(
                new_user.Id, new_user.UserName, FakeConfigGetToken.KEY, out DateTime time);
            var exisiting_refresh_token = GenerateAuthTokensAction.CreateRefreshToken(auth_key);

            var dto = new TokensRequestDto()
            {
                AuthToken = auth_key,
                RefreshToken = exisiting_refresh_token.Token
            };

            //ACT
            var result = await service.RefreshTokens(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            config.Verify();
        }

        [Test]
        public async Task Refresh_UserDoesntHaveProvidedRefreshToken_Null()
        {
            //ARRANGE
            var config = new FakeConfigGetToken();

            var service = new AuthRefreshTokensService(
                MapperHelper.GetNewInstance(),
                config.Object,
                _unitOfWork
                );

            var new_user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            string auth_key = GenerateAuthTokensAction.CreateAuthToken(
                new_user.Id, new_user.UserName, FakeConfigGetToken.KEY, out DateTime time);
            var exisiting_refresh_token = GenerateAuthTokensAction.CreateRefreshToken(auth_key);

            _unitOfWork.Context.Users.Add(new_user);
            _unitOfWork.Context.SaveChanges();

            var dto = new TokensRequestDto()
            {
                AuthToken = auth_key,
                RefreshToken = exisiting_refresh_token.Token
            };

            //ACT
            var result = await service.RefreshTokens(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            config.Verify();
        }

        [Test]
        public async Task Refresh_ProvidedAuthTokenNotMatch_Null()
        {
            //ARRANGE
            var config = new FakeConfigGetToken();

            var service = new AuthRefreshTokensService(
                MapperHelper.GetNewInstance(),
                config.Object,
                _unitOfWork
                );

            var new_user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            string auth_key = GenerateAuthTokensAction.CreateAuthToken(
                new_user.Id, new_user.UserName, FakeConfigGetToken.KEY, out DateTime time);
            string not_matching_auth_key = GenerateAuthTokensAction.CreateAuthToken(
                new_user.Id, new_user.UserName + "change", FakeConfigGetToken.KEY, out DateTime time2);
            var exisiting_refresh_token = GenerateAuthTokensAction.CreateRefreshToken(auth_key);

            var tokens = new List<RefreshToken>();
            tokens.Add(exisiting_refresh_token);
            new_user.RefreshTokens = tokens;

            _unitOfWork.Context.Users.Add(new_user);
            _unitOfWork.Context.SaveChanges();

            var dto = new TokensRequestDto()
            {
                AuthToken = not_matching_auth_key,
                RefreshToken = exisiting_refresh_token.Token
            };

            //ACT
            var result = await service.RefreshTokens(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            config.Verify();
        }

        [Test]
        public async Task Refresh_RefreshTokenWasAlreadyUsed_Null()
        {
            //ARRANGE
            var config = new FakeConfigGetToken();

            var service = new AuthRefreshTokensService(
                MapperHelper.GetNewInstance(),
                config.Object,
                _unitOfWork
                );

            var new_user = new ApiUser()
            {
                Id = "0",
                UserName = "username"
            };

            string auth_key = GenerateAuthTokensAction.CreateAuthToken(
                new_user.Id, new_user.UserName, FakeConfigGetToken.KEY, out DateTime time);
            var exisiting_refresh_token = GenerateAuthTokensAction.CreateRefreshToken(auth_key);

            var tokens = new List<RefreshToken>();
            exisiting_refresh_token.Revoked = DateTime.MinValue;
            tokens.Add(exisiting_refresh_token);
            new_user.RefreshTokens = tokens;

            _unitOfWork.Context.Users.Add(new_user);
            _unitOfWork.Context.SaveChanges();

            var dto = new TokensRequestDto()
            {
                AuthToken = auth_key,
                RefreshToken = exisiting_refresh_token.Token
            };

            //ACT
            var result = await service.RefreshTokens(dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
            config.Verify();
        }
    }
}

