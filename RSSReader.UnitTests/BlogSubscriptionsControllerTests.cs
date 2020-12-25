using NUnit.Framework;
using RSSReader.Dtos;
using System.Threading.Tasks;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class BlogSubscriptionsControllerTests
    {
        BlogSubscriptionForAddDto _blogSubscriptionForAddDto;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            
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
