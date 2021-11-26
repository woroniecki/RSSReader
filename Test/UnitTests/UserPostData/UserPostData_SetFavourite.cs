using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Tests.Helpers;

namespace Tests.UnitTests
{
    [TestFixture]
    class UserPostData_SetFavourite
    {
        private DataContext _context;
        private UnitOfWork _unitOfWork;

        [SetUp]
        public void SetUp()
        {
            var options = InMemoryDb.CreateNewContextOptions();
            _context = new DataContext(options);
            _unitOfWork = new UnitOfWork(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Dispose();
        }

        [Test]
        public async Task Set_favourite_to_true_and_increase_fav_amount_in_post_Success()
        {
            //ARRANGE
            var user_post_data = new UserPostData();
            var post = new Post();
            user_post_data.Post = post;
            post.FavouriteAmount = 0;
            user_post_data.Favourite = false;

            //ACT
            user_post_data.SetFavourite(true);

            //ASSERT
            Assert.IsTrue(user_post_data.Favourite);
            Assert.That(post.FavouriteAmount, Is.EqualTo(1));
        }

        [Test]
        public async Task Set_favourite_to_false_and_decrease_fav_amount_in_post_Success()
        {
            //ARRANGE
            var user_post_data = new UserPostData();
            var post = new Post();
            user_post_data.Post = post;
            post.FavouriteAmount = 1;
            user_post_data.Favourite = true;

            //ACT
            user_post_data.SetFavourite(false);

            //ASSERT
            Assert.IsFalse(user_post_data.Favourite);
            Assert.That(post.FavouriteAmount, Is.EqualTo(0));
        }

        [Test]
        public async Task Fav_amount_cant_go_below_zero_in_post_Success()
        {
            //ARRANGE
            var user_post_data = new UserPostData();
            var post = new Post();
            user_post_data.Post = post;
            post.FavouriteAmount = 0;
            user_post_data.Favourite = true;

            //ACT
            user_post_data.SetFavourite(false);

            //ASSERT
            Assert.IsFalse(user_post_data.Favourite);
            Assert.That(post.FavouriteAmount, Is.EqualTo(0));
        }
    }
}
