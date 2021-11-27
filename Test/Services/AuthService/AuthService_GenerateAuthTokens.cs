using DataLayer.Models;
using LogicLayer.Helpers;
using NUnit.Framework;
using ServiceLayer.AuthServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tests.Fake;

namespace Tests.UnitTests
{
    [TestFixture]
    class AuthService_GenerateAuthTokens
    {
        private IAuthService _authService;

        [SetUp]
        public void SetUp()
        {
            var config = new FakeConfigGetToken();
            _authService = new AuthService(config.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task Generate_new_tokens__Tokens_with_correct_expire_date()
        {
            //ARRANGE
            string USER_ID = "USER_ID";
            ApiUser user = new ApiUser()
            {
                Id = USER_ID,
                UserName = "Name",
                RefreshTokens = new List<RefreshToken>()
            };
            var start_time = DateTime.UtcNow;

            //ACT
            var tokens = _authService.GenerateAuthTokens(user);

            //ASSERT
            Assert.That(tokens.AuthToken.Expires, Is.GreaterThan(start_time.From1970()));
            Assert.That(tokens.RefreshToken.Expires, Is.GreaterThan(start_time.From1970()));
            Assert.IsFalse(string.IsNullOrEmpty(tokens.AuthToken.Token));
            Assert.IsFalse(string.IsNullOrEmpty(tokens.RefreshToken.Token));
        }
    }
}
