using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess._const;
using DbAccess.Core;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ServiceLayer.PostServices;
using Tests.Helpers;

namespace Tests.Services.PostServices
{
    [TestFixture]
    class PostListServiceTest
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
        public async Task GetList_HappyPathWithUpdateBlog_ListOfGroups()
        {
            //ARRANGE
            var user1 = new ApiUser() { UserName = "user1" };

            var blog = new Blog() { Url = "www.url.com", LastPostsRefreshDate = DateTime.UtcNow.AddDays(-1) };
            _context.Add(blog);
            _context.Add(user1);
            _unitOfWork.Context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService().GetStringContentFromFile(blog.Url, "../../../Data/feeddata.xml");
            var service = new PostListService
                (
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    httpHelperService.Object
                );

            var start_time = DateTime.UtcNow;
            //ACT
            var result = await service.GetList(user1.Id, blog.Id, 0);

            //ASSERT
            Assert.That(result.Count, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(result.Where(x => !x.Favourite).Count, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(result.Where(x => !x.Readed).Count, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            var updated_blog = _context.Blogs.Include(x => x.Posts).Where(x => x.Id == blog.Id).FirstOrDefault();
            Assert.That(updated_blog.Posts.Count, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(updated_blog.LastPostsRefreshDate, Is.GreaterThanOrEqualTo(start_time));
            httpHelperService.Verify();
        }

        [Test]
        public async Task GetList_HappyPathWithoutUpdateBlog_ListOfGroups()
        {
            //ARRANGE
            var user1 = new ApiUser() { UserName = "user1" };
            var user2 = new ApiUser() { UserName = "user2" };
            var blog = new Blog() { LastPostsRefreshDate = DateTime.UtcNow };
            var post1 = new Post() { Blog = blog };
            var post2 = new Post() { Blog = blog };
            var post3 = new Post() { Blog = blog };
            var post1_user1 = new UserPostData() { User = user1, Post = post1, Readed = true, Favourite = false };
            var post2_user1 = new UserPostData() { User = user1, Post = post2, Readed = false, Favourite = true };
            _context.Add(user1);
            _context.Add(user2);
            _context.Add(blog);
            _context.Add(post1);
            _context.Add(post2);
            _context.Add(post3);
            _context.Add(post1_user1);
            _context.Add(post2_user1);
            _unitOfWork.Context.SaveChanges();

            var service = new PostListService
                (
                    MapperHelper.GetNewInstance(), 
                    _unitOfWork,
                    new FakeHttpHelperService().Object
                );

            //ACT
            var result = await service.GetList(user1.Id, blog.Id, 0);

            //ASSERT
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.Where(x => x.Favourite).Count, Is.EqualTo(1));
            Assert.That(result.Where(x => !x.Favourite).Count, Is.EqualTo(2));
            Assert.That(result.Where(x => x.Readed).Count, Is.EqualTo(1));
            Assert.That(result.Where(x => !x.Readed).Count, Is.EqualTo(2));
            Assert.NotNull(result.Where(x => x.Id == post1.Id).First());
            Assert.NotNull(result.Where(x => x.Id == post2.Id).First());
        }

        [Test]
        public async Task GetList_CantFindUserWithProvidedId_Null()
        {
            //ARRANGE
            var blog = new Blog() { LastPostsRefreshDate = DateTime.UtcNow };
            _context.Add(blog);
            _context.SaveChanges();

            var service = new PostListService
                (
                    MapperHelper.GetNewInstance(), 
                    _unitOfWork,
                    new FakeHttpHelperService().Object
                );

            //ACT
            var result = await service.GetList("0", blog.Id, 0);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetList_CantFindBlog_Null()
        {
            //ARRANGE
            _unitOfWork.Context.SaveChanges();

            var service = new PostListService
                (
                    MapperHelper.GetNewInstance(), 
                    _unitOfWork,
                    new FakeHttpHelperService().Object
                );

            //ACT
            var result = await service.GetList("0", 0, 0);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }
    }
}
