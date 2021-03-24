using NUnit.Framework;
using RSSReader.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class FeedServiceTests
    {
        private FeedService _feedService;
        private string _url;

        [SetUp]
        public void SetUp()
        {
            _feedService = new FeedService();
            _url = "www.url.com";
        }

        #region CreateBlogObject
        [Test]
        public void CreateBlogObject_Ok_CreatedBlogWithAllInfoFields()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata.xml"))
            {
                feed_data = r.ReadToEnd();
            }

            //ACT
            var feed_list = _feedService.ParseFeed(feed_data);
            var result = _feedService.CreateBlogObject(_url, feed_data, feed_list);

            //ASSERT
            Assert.IsFalse(string.IsNullOrEmpty(result.Name));
            Assert.IsFalse(string.Equals(_url, result.Name));//should take main title node from feed
            Assert.IsFalse(string.IsNullOrEmpty(result.Description));
            Assert.IsFalse(string.IsNullOrEmpty(result.ImageUrl));
        }

        [Test]
        public void CreateBlogObject_Ok_CreatedBlogDescriptionFromSubtitle()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata_descriptionfromsubtitle.xml"))
            {
                feed_data = r.ReadToEnd();
            }

            //ACT
            var feed_list = _feedService.ParseFeed(feed_data);
            var result = _feedService.CreateBlogObject(_url, feed_data, feed_list);

            //ASSERT
            Assert.IsFalse(string.Equals(_url, result.Name));//should take main title node from feed
            Assert.IsFalse(string.IsNullOrEmpty(result.Description));
        }
        #endregion
    }
}
