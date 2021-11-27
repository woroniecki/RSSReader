using DataLayer.Code;
using DataLayer.Models;
using NUnit.Framework;
using System;
using Tests.Helpers;

namespace Tests.UnitTests
{
    [TestFixture]
    class Subscription_SetGroup
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
        public void SetGroup_SetNewGroup_Sucess()
        {
            //ARRANGE
            string USER_ID = "user_id";
            var sub = new Subscription(USER_ID, new Blog());
            var user = new ApiUser() { Id = USER_ID };
            var group_to_set = new Group() { Id = 1, User = user };

            //ACT
            sub.SetGroup(USER_ID, group_to_set);

            //ASSERT
            Assert.That(sub.Group, Is.EqualTo(group_to_set));
            Assert.That(sub.GroupId.Value, Is.EqualTo(group_to_set.Id));
        }

        public void SetGroup_RemovesGroup_Sucess()
        {
            //ARRANGE
            string USER_ID = "user_id";
            var sub = new Subscription(USER_ID, new Blog());
            Group group_to_set = null;

            //ACT
            sub.SetGroup(USER_ID, group_to_set);

            //ASSERT
            Assert.IsNull(sub.Group);
            Assert.IsFalse(sub.GroupId.HasValue);
        }

        [Test]
        public void SetGroup_UserNotOwnsSub_Exception()
        {
            //ARRANGE
            string USER_ID = "user_id", WRONG_USER_ID = "wrong_user_id";
            var sub = new Subscription(USER_ID, new Blog());
            var group_to_set = new Group() { Id = 1 };

            //ACT
            var ex = Assert.Throws<Exception>(() => sub.SetGroup(WRONG_USER_ID, group_to_set));

            //ASSERT
            Assert.That(ex.Message, Is.EqualTo("Unauthorized."));
        }

        [Test]
        public void SetGroup_UserNotOwnsGroup_Exception()
        {
            //ARRANGE
            string USER_ID = "user_id", WRONG_USER_ID = "wrong_user_id";
            var sub = new Subscription(USER_ID, new Blog());
            var user = new ApiUser() { Id = WRONG_USER_ID };
            var group_to_set = new Group() { Id = 1, User = user };

            //ACT
            var ex = Assert.Throws<Exception>(() => sub.SetGroup(USER_ID, group_to_set));

            //ASSERT
            Assert.That(ex.Message, Is.EqualTo("Unauthorized."));
        }
    }
}
