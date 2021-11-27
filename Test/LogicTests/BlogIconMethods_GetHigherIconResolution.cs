using System.Drawing;
using System.Threading.Tasks;
using DataLayer.Code;
using LogicLayer.Helpers;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.LogicTests
{
    [TestFixture]
    class BlogIconMethods_GetHigherIconResolution
    {
        private DataContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = InMemoryDb.CreateNewContextOptions();
            _context = new DataContext(options);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task GetHigherIconResolution_FoundHigherIconRes_Newurl()
        {
            //ARRANGE
            string high_res_url = "https://rogbarana.pl/wp-content/uploads/2020/12/cropped-cropped-stol2-150x150.jpg";
            string url = "https://rogbarana.pl/wp-content/uploads/2020/12/cropped-cropped-stol2-32x32.jpg";


            var httpService = new FakeHttpHelperService()
                .GetImageContent(high_res_url, new Bitmap(1, 1));

            //ACT
            var result = await BlogIconMethods.GetHigherIconResolution(url, httpService.Object);

            //ASSERT
            Assert.That(result, Is.EqualTo(high_res_url));
            httpService.Verify();
        }

        [Test]
        public async Task GetHigherIconResolution_No32x32InUrl_Sameurl()
        {
            //ARRANGE
            string url = "https://rogbarana.pl/wp-content/uploads/2020/12/cropped-cropped-stol2.jpg";

            var httpService = new FakeHttpHelperService(); ;

            //ACT
            var result = await BlogIconMethods.GetHigherIconResolution(url, httpService.Object);

            //ASSERT
            Assert.That(result, Is.EqualTo(url));
        }

        [Test]
        public async Task GetHigherIconResolution_NoContentAfterReplace32x32_Sameurl()
        {
            //ARRANGE
            string high_res_url = "https://rogbarana.pl/wp-content/uploads/2020/12/cropped-cropped-stol2-150x150.jpg";
            string url = "https://rogbarana.pl/wp-content/uploads/2020/12/cropped-cropped-stol2-32x32.jpg";

            var httpService = new FakeHttpHelperService()
                .GetImageContent(high_res_url, null);

            //ACT
            var result = await BlogIconMethods.GetHigherIconResolution(url, httpService.Object);

            //ASSERT
            Assert.That(result, Is.EqualTo(url));
            httpService.Verify();
        }

        [Test]
        public async Task GetHigherIconResolution_UrlIsNull_Null()
        {
            //ARRANGE
            string url = null;

            var httpService = new FakeHttpHelperService();

            //ACT
            var result = await BlogIconMethods.GetHigherIconResolution(url, httpService.Object);

            //ASSERT
            Assert.IsNull(result);
        }
    }
}
