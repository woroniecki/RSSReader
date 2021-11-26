using DataLayer.Models;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tests.UnitTests
{
    [TestFixture]
    class UserPostData_SetReaded
    {

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task Changes_readed_flag_to_true_set_last_date_open_Success()
        {
            //ARRANGE
            var user_post_data = new UserPostData();
            user_post_data.Readed = false;
            var start_time = DateTime.UtcNow;

            //ACT
            user_post_data.SetReaded(true);

            //ASSERT
            Assert.IsTrue(user_post_data.Readed);
            Assert.That(user_post_data.LastDateOpen, Is.GreaterThan(start_time));
        }

        [Test]
        public async Task Changes_readed_flag_to_false_ignore_last_date_open_Success()
        {
            //ARRANGE
            var user_post_data = new UserPostData();
            user_post_data.Readed = true;
            var start_time = DateTime.UtcNow;
            user_post_data.LastDateOpen = start_time;

            //ACT
            user_post_data.SetReaded(false);

            //ASSERT
            Assert.IsFalse(user_post_data.Readed);
            Assert.That(user_post_data.LastDateOpen, Is.EqualTo(start_time));
        }
    }
}
