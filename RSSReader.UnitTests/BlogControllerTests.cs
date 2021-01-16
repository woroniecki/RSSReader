using NUnit.Framework;
using RSSReader.Controllers;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using static RSSReader.Data.UserRepository;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class BlogControllerTests
    {
        private BlogController _blogController;

        [SetUp]
        public void SetUp()
        {
            _blogController = new BlogController();
        }

        #region GetUserPostDataList
        [Test]
        public async Task GetUserPostDataList_GetList_ListWithData()
        {
            //ARRANGE

            //ACT
            var result = await _blogController.GetUserPostDataList();

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
        }
        
        #endregion
    }
}
