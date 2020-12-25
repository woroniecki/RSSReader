using NUnit.Framework;
using System.Threading.Tasks;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class BlogSubscriptionsControllerTests
    {

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

        #endregion
    }
}
