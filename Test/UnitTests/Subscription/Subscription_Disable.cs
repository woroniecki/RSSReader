using DataLayer.Models;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tests.UnitTests
{
    [TestFixture]
    class Subscription_Disable
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
        public void Disable_Unsubscribe_Ok()
        {
            //ARRANGE
            string USER_ID = "user_id";
            var sub = new Subscription(USER_ID, new Blog());
            var time = DateTime.UtcNow;

            //ACT
            sub.Disable(USER_ID);

            //ASSERT
            Assert.IsFalse(sub.Active);
            Assert.Greater(sub.LastUnsubscribeDate, time);
        }

        [Test]
        public void Disable_Unauthorized_Exception()
        {
            //ARRANGE
            string USER_ID = "user_id", WRONG_USER_ID = "wrong_user_id";
            var sub = new Subscription(USER_ID, new Blog());
            var time = DateTime.UtcNow;

            //ACT
            var ex = Assert.Throws<Exception> (() => sub.Disable(WRONG_USER_ID));

            //ASSERT
            Assert.That(ex.Message, Is.EqualTo("Unauthorized."));
        }

        [Test]
        public void Disable_AlreadyDisabled_Exception()
        {
            //ARRANGE
            string USER_ID = "user_id";
            var sub = new Subscription(USER_ID, new Blog());

            //ACT
            sub.Disable(USER_ID);//Disable sub
            var ex = Assert.Throws<Exception>(() => sub.Disable(USER_ID));//And try when is already disabled

            //ASSERT
            Assert.That(ex.Message, Is.EqualTo("Subscription is already disabled"));
        }
    }
}
