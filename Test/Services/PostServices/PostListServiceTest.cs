using System;
using System.Linq;
using System.Threading.Tasks;
using LogicLayer._const;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ServiceLayer.PostServices;
using Tests.Helpers;
using System.Collections.Generic;

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
            Assert.That(result.Where(x => !x.UserData.Favourite).Count, Is.EqualTo(RssConsts.POSTS_PER_CALL));
            Assert.That(result.Where(x => !x.UserData.Readed).Count, Is.EqualTo(RssConsts.POSTS_PER_CALL));
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
            Assert.That(result.Where(x => x.UserData.Favourite).Count, Is.EqualTo(1));
            Assert.That(result.Where(x => !x.UserData.Favourite).Count, Is.EqualTo(2));
            Assert.That(result.Where(x => x.UserData.Readed).Count, Is.EqualTo(1));
            Assert.That(result.Where(x => !x.UserData.Readed).Count, Is.EqualTo(2));
            Assert.NotNull(result.Where(x => x.Id == post1.Id).First());
            Assert.NotNull(result.Where(x => x.Id == post2.Id).First());
        }

        [Test]
        public async Task GetList_HappyPathUserNotLoggedIn_ListWithoutUserData()
        {
            //ARRANGE
            var blog = new Blog() { LastPostsRefreshDate = DateTime.UtcNow };
            var post1 = new Post() { Blog = blog };
            var post2 = new Post() { Blog = blog };
            var post3 = new Post() { Blog = blog };
            _context.Add(blog);
            _context.Add(post1);
            _context.Add(post2);
            _context.Add(post3);
            _unitOfWork.Context.SaveChanges();

            var service = new PostListService
                (
                    MapperHelper.GetNewInstance(),
                    _unitOfWork,
                    new FakeHttpHelperService().Object
                );

            //ACT
            var result = await service.GetList("", blog.Id, 0);

            //ASSERT
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.NotNull(result.Where(x => x.Id == post1.Id).First());
            Assert.NotNull(result.Where(x => x.Id == post2.Id).First());
        }

        [Test]
        public async Task GetList_HalfPostsAreNew_ListOfGroups()
        {
            //ARRANGE
            const int POSTS_AMOUNT_IN_FILE = 4;
            var user1 = new ApiUser() { UserName = "user1" };

            var blog = new Blog() { Url = "www.url.com", LastPostsRefreshDate = DateTime.UtcNow.AddDays(-1) };
            blog.Posts = new List<Post>();
            for(int i = 3; i < 5; i++)
                blog.Posts.Add(new Post() { Name = $"Title{i}", AddedDate = DateTime.UtcNow.AddSeconds(-i) });

            _context.Add(blog);
            _context.Add(user1);
            _unitOfWork.Context.SaveChanges();

            int id_post_added_before_update = blog.Posts.Last().Id;

            var httpHelperService = new FakeHttpHelperService().GetStringContentFromFile(blog.Url, "../../../Data/feeddata_update_test.xml");
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
            Assert.That(result.Count, Is.EqualTo(POSTS_AMOUNT_IN_FILE), "Should add 2 blogs and remove 2 old ones");
            Assert.That(result.ElementAt(0).Name, Is.EqualTo("Title1"), "Verify if new post were added");
            Assert.That(result.Last().Id, Is.EqualTo(id_post_added_before_update), "Verify that last post weren't deleted");
        }

        [Test]
        public async Task GetList_NonePostIsNewSoDontAddAny_ListOfGroups()
        {
            //ARRANGE
            const int POSTS_AMOUNT_IN_FILE = 4;
            var user1 = new ApiUser() { UserName = "user1" };

            var blog = new Blog() { Url = "www.url.com", LastPostsRefreshDate = DateTime.UtcNow.AddDays(-1) };
            blog.Posts = new List<Post>();
            for (int i = 1; i < POSTS_AMOUNT_IN_FILE + 1; i++)
                blog.Posts.Add(new Post() { Name = $"Title{i}", AddedDate = DateTime.UtcNow.AddSeconds(-i) });

            _context.Add(blog);
            _context.Add(user1);
            _unitOfWork.Context.SaveChanges();

            int id_post_added_before_update_first = blog.Posts.First().Id;
            int id_post_added_before_update_last = blog.Posts.Last().Id;

            var httpHelperService = new FakeHttpHelperService().GetStringContentFromFile(blog.Url, "../../../Data/feeddata_update_test.xml");
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
            Assert.That(result.Count, Is.EqualTo(POSTS_AMOUNT_IN_FILE), "Should add 2 blogs and remove 2 old ones");
            Assert.That(result.ElementAt(0).Id, Is.EqualTo(id_post_added_before_update_first), "Verify that first post weren't deleted");
            Assert.That(result.Last().Id, Is.EqualTo(id_post_added_before_update_last), "Verify that last post weren't deleted");
        }

        [Test]
        public async Task GetList_AllPostsAreNewSoAddAllAndAllWhichAreNotFavourite_ListOfGroups()
        {
            //ARRANGE
            const int POSTS_AMOUNT_IN_FILE = 4;
            var user1 = new ApiUser() { UserName = "user1" };

            var blog = new Blog() { Url = "www.url.com", LastPostsRefreshDate = DateTime.UtcNow.AddDays(-1) };
            blog.Posts = new List<Post>();
            for (int i = 5; i < 5 + POSTS_AMOUNT_IN_FILE; i++)
                blog.Posts.Add(new Post() { Name = $"Title{i}", AddedDate = DateTime.UtcNow.AddSeconds(-i) });
            blog.Posts.Last().FavouriteAmount = 1;

            _context.Add(blog);
            _context.Add(user1);
            _unitOfWork.Context.SaveChanges();

            var httpHelperService = new FakeHttpHelperService().GetStringContentFromFile(blog.Url, "../../../Data/feeddata_update_test.xml");
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
            Assert.That(result.Count, Is.EqualTo(POSTS_AMOUNT_IN_FILE + 1), "Should add 4 blogs and keep one which is favourite");
            Assert.That(result.First().Name, Is.EqualTo("Title1"), "Verify if new post were added");
            Assert.That(result.ElementAt(POSTS_AMOUNT_IN_FILE - 1).Name, Is.EqualTo("Title4"), "Verify if new post were added");
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
            Assert.Null(result);
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
