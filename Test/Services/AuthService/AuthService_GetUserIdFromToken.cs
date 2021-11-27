using DataLayer.Models;
using NUnit.Framework;
using ServiceLayer.AuthServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tests.Fake;

namespace Tests.UnitTests
{
    [TestFixture]
    class AuthService_GetUserIdFromToken
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
        public async Task Correctly_decode_token_and_finds_user_id__Returns_user_id()
        {
            //ARRANGE
            string USER_ID = "USER_ID";
            ApiUser user = new ApiUser() { 
                Id = USER_ID, 
                UserName = "Name",
                RefreshTokens = new List<RefreshToken>()
            };
            string token = _authService.GenerateAuthTokens(user).AuthToken.Token;

            //ACT
            string result_id = _authService.GetUserIdFromToken(token);

            //ASSERT
            Assert.That(result_id, Is.EqualTo(USER_ID));
        }

        [Test]
        public async Task Try_to_get_id_from_broken_token__Throw_exception()
        {
            //ARRANGE
            string USER_ID = "USER_ID";
            ApiUser user = new ApiUser()
            {
                Id = USER_ID,
                UserName = "Name",
                RefreshTokens = new List<RefreshToken>()
            };
            string token = "Invalid.Token.Key";

            //ACT
            var ex = Assert.Throws<ArgumentException>(() => _authService.GetUserIdFromToken(token));
        }
    }
}
