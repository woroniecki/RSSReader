using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using RSSReader.Dtos;
using System.Threading.Tasks;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class BlogSubscriptionsControllerTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        BlogSubscriptionForAddDto _blogSubscriptionForAddDto;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
                );

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

            //ACT

            //ASSERT
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
