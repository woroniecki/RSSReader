using DataLayer.Models;
using Dtos.Subscriptions;
using NUnit.Framework;
using System;

namespace Tests.UnitTests
{
    [TestFixture]
    class Subscription_Update
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Update_ChangesReadFiltering_ValueChanged()
        {
            //ARRANGE
            string USER_ID = "user_id";
            var sub = new Subscription(USER_ID, new Blog());
            sub.FilterReaded = false;
            var dto = new UpdateSubscriptionRequestDto() {
                FilterReaded = true
            };

            //ACT
            sub.Update(USER_ID, dto);

            //ASSERT
            Assert.IsTrue(sub.FilterReaded);
        }

        [Test]
        public void Update_Unauthorized_Exception()
        {
            //ARRANGE
            string USER_ID = "user_id", WRONG_USER_ID = "wrong_user_id";
            var sub = new Subscription(USER_ID, new Blog());
            var time = DateTime.UtcNow;
            var dto = new UpdateSubscriptionRequestDto();

            //ACT
            var ex = Assert.Throws<Exception>(() => sub.Update(WRONG_USER_ID, dto));

            //ASSERT
            Assert.That(ex.Message, Is.EqualTo("Unauthorized."));
        }
    }
}
