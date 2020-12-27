using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using RSSReader.Controllers;
using RSSReader.Dtos;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class BlogSubscriptionsControllerTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private Mock<BlogSubscriptionsController> _blogSubscriptionsControllerMock;
        BlogSubscriptionForAddDto _blogSubscriptionForAddDto;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
                );

            _blogSubscriptionsControllerMock = new Mock<BlogSubscriptionsController>(
                _userManagerMock.Object
                );
            _blogSubscriptionsControllerMock.CallBase = true;

            //Dto
            _blogSubscriptionForAddDto = new Dtos.BlogSubscriptionForAddDto()
            {
                BlogUrl = "Http://blog.com"
            };
        }

        #region AddBlogSubscription

        [Test]
        public async Task AddBlogSubscription_CreateBlogSubAndBlogRowInDB_CreatedResult()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        [Test]
        public async Task AddBlogSubscription_CreatesBlogSubWithAlreadyExisitingBlogRow_CreatedResult()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        [Test]
        public async Task AddBlogSubscription_EnablesAlreadyExistingBlogsub_Ok()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        [Test]
        public async Task AddBlogSubscription_CantFindUserFromClaims_Unauthorized()
        {
            //ARRANGE
            _blogSubscriptionsControllerMock.Protected()
                .Setup<Task<IdentityUser>>("GetCurrentUser")
                .Returns(Task.FromResult<IdentityUser>(null))
                .Verifiable();

            //ACT
            var result = await _blogSubscriptionsControllerMock.Object.GetSubscribedBlogsList();

            //ASSERT
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task AddBlogSubscription_InvalidBlogUrl_BadRequest()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        [Test]
        public async Task AddBlogSubscription_BlogWithDeliveredUrlNotExists_BadRequest()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        #endregion
    }
}
