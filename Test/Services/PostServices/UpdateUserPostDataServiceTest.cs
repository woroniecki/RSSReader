using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.UserPostData;
using Moq;
using NUnit.Framework;
using ServiceLayer.PostServices;
using Tests.Helpers;

namespace Tests.Services.PostServices
{
    [TestFixture]
    class UpdateUserPostDataServiceTest
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
        public async Task Update_UpdateWhenUserPostDateDoesntExist_PostResponseDto()
        {
            //ARRANGE
            var user = new ApiUser() { };
            _context.Add(user);

            var blog = new Blog() { };
            _context.Add(blog);

            var post = new Post() { Blog = blog };
            _context.Add(post);

            var sub = new Subscription(user.Id, blog);
            _context.Add(sub);

            _context.SaveChanges();

            var inData = new UpdateUserPostDataRequestDto() { Readed = true };

            var service = new UpdateUserPostDataService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.Update(inData, post.Id, user.Id);

            //ASSERT
            Assert.That(result.Id, Is.EqualTo(post.Id));
            Assert.That(result.UserData.Readed, Is.EqualTo(inData.Readed));
            Assert.That(result.UserData.Favourite, Is.EqualTo(false));
            var update_upd = _context.UserPostDatas.Where(x => x.Post.Id == post.Id && x.User.Id == user.Id).FirstOrDefault();
            Assert.That(update_upd.Readed, Is.EqualTo(inData.Readed));
            Assert.That(update_upd.Favourite, Is.EqualTo(false));
        }

        [Test]
        public async Task Update_UpdateReadedExistingUserPostData_PostResponseDto()
        {
            //ARRANGE
            var user = new ApiUser() { };
            _context.Add(user);

            var blog = new Blog() { };
            _context.Add(blog);

            var post = new Post() { Blog = blog };
            _context.Add(post);

            var sub = new Subscription(user.Id, blog);
            _context.Add(sub);

            var user_post_data = new UserPostData()
            {
                User = user,
                Post = post,
                Subscription = sub,
                Readed = true, 
                Favourite = false 
            };
            _context.Add(user_post_data);

            _context.SaveChanges();

            var inData = new UpdateUserPostDataRequestDto() { Readed = false };

            var service = new UpdateUserPostDataService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.Update(inData, post.Id, user.Id);

            //ASSERT
            Assert.That(result.Id, Is.EqualTo(post.Id));
            Assert.That(result.UserData.Readed, Is.EqualTo(inData.Readed));
            Assert.That(result.UserData.Favourite, Is.EqualTo(false));
            var update_upd = _context.UserPostDatas.Where(x => x.Id == user_post_data.Id).FirstOrDefault();
            Assert.IsNotNull(update_upd);
            Assert.That(update_upd.Readed, Is.EqualTo(inData.Readed));
            Assert.That(update_upd.Favourite, Is.EqualTo(false));
        }

        [Test]
        public async Task Update_UpdateAllExistingUserPostData_PostResponseDto()
        {
            //ARRANGE
            var user = new ApiUser() { };
            _context.Add(user);

            var blog = new Blog() { };
            _context.Add(blog);

            var post = new Post() { Blog = blog };
            _context.Add(post);

            var sub = new Subscription(user.Id, blog);
            _context.Add(sub);

            var user_post_data = new UserPostData() { 
                User = user, 
                Post = post,
                Subscription = sub,
                Readed = true, 
                Favourite = true 
            };
            _context.Add(user_post_data);

            _context.SaveChanges();

            var inData = new UpdateUserPostDataRequestDto() { Readed = false, Favourite = true };

            var service = new UpdateUserPostDataService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.Update(inData, post.Id, user.Id);

            //ASSERT
            Assert.That(result.Id, Is.EqualTo(post.Id));
            Assert.That(result.UserData.Readed, Is.EqualTo(inData.Readed));
            Assert.That(result.UserData.Favourite, Is.EqualTo(inData.Favourite));
            var update_upd = _context.UserPostDatas.Where(x => x.Id == user_post_data.Id).FirstOrDefault();
            Assert.IsNotNull(update_upd);
            Assert.That(update_upd.Readed, Is.EqualTo(inData.Readed));
            Assert.That(update_upd.Favourite, Is.EqualTo(inData.Favourite));
        }

        [Test]
        public async Task Update_CantFindUser_Null()
        {
            //ARRANGE
            var post = new Post() { };
            _context.Add(post);

            _context.SaveChanges();

            var inData = new UpdateUserPostDataRequestDto();

            var service = new UpdateUserPostDataService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.Update(inData, post.Id, "0");

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Update_CantFindPost_Null()
        {
            //ARRANGE
            var user = new ApiUser() { };
            _context.Add(user);

            _context.SaveChanges();

            var inData = new UpdateUserPostDataRequestDto();

            var service = new UpdateUserPostDataService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.Update(inData, 0, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Update_CantFindSub_Null()
        {
            //ARRANGE
            var user = new ApiUser() { };
            _context.Add(user);

            var blog = new Blog() { };
            _context.Add(blog);

            var post = new Post() { Blog = blog };
            _context.Add(post);

            _context.SaveChanges();

            var inData = new UpdateUserPostDataRequestDto();

            var service = new UpdateUserPostDataService(MapperHelper.GetNewInstance(), _unitOfWork);

            //ACT
            var result = await service.Update(inData, post.Id, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }
    }
}
