using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RSSReader.Controllers;
using RSSReader.Data;
using RSSReader.Models;
using RSSReader.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using static RSSReader.Data.Repositories.UserRepository;
using UserPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.ApiUser, bool>>;
using BlogPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.Blog, bool>>;
using PostPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.Post, bool>>;
using UserPostDataPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.UserPostData, bool>>;
using RSSReader.Dtos;
using RSSReader.Data.Repositories;
using Microsoft.Toolkit.Parsers.Rss;
using AutoWrapper.Wrappers;
using System.IO;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class BlogControllerTests
    {
        const int BLOG_ID = 1;

        private BlogController _blogController;
        private Mock<IBlogRepository> _blogRepo;
        private Mock<IPostRepository> _postRepo;
        private Mock<IUserPostDataRepository> _userPostDataRepo;
        private Mock<IUserRepository> _userRepo;
        private Mock<IReaderRepository> _readerRepo;
        private Mock<FeedService> _feedService;
        private List<UserPostData> _resultList;
        private ApiUser _user;
        private Blog _blog;
        private Post _post;
        private UserPostData _userPostData;
        private DataForReadPostDto _readPostModel;

        [SetUp]
        public void SetUp()
        {
            //Mock
            _blogRepo = new Mock<IBlogRepository>();
            _postRepo = new Mock<IPostRepository>();
            _userPostDataRepo = new Mock<IUserPostDataRepository>();
            _userRepo = new Mock<IUserRepository>();
            _readerRepo = new Mock<IReaderRepository>();
            _feedService = new Mock<FeedService>()
            {
                CallBase = true
            };

            //Data
            _resultList = Enumerable.Repeat(
                new UserPostData() { }, 1)
                .ToList();
            _user = new ApiUser()
            {
                Id = "2",
                UserName = "username",
                Email = "user@mail.com"
            };
            _blog = new Blog()
            {
                Id = 1,
                Url = "itsnotworkingbloglink.com"
            };
            _readPostModel = new DataForReadPostDto()
            {
                Name = "name",
                PostUrl = "www.test.com/1"
            };
            _post = new Post()
            {
                Id = 1,
                Name = _readPostModel.Name,
                Url = _readPostModel.PostUrl,
                Blog = _blog,
            };
            _userPostData = new UserPostData(_post, _user);

            //Controller
            _blogController = new BlogController(
                _readerRepo.Object,
                _blogRepo.Object,
                _postRepo.Object,
                _userPostDataRepo.Object,
                _userRepo.Object,
                _feedService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                    new Claim(ClaimTypes.NameIdentifier, _user.Id)
                               }, "TestAuthentication"));
            _blogController.ControllerContext = new ControllerContext();
            _blogController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        #region Mock

        private void Mock_UserPostDataRepository_GetListWithPosts(
            IEnumerable<UserPostData> returnedList)
        {
            Expression<Func<IUserPostDataRepository, Task<IEnumerable<UserPostData>>>> expression =
                x => x.GetListWithPosts(It.IsAny<UserPostDataPred>());

            _userPostDataRepo.Setup(expression)
            .Returns(Task.FromResult(returnedList))
            .Verifiable();
        }

        private void Mock_UserRepository_Get(ApiUser returnedUser)
        {
            Expression<Func<IUserRepository, Task<ApiUser>>> expression =
                returnedUser != null ?
                x => x.Get(It.Is<UserPred>(x => x.Compile().Invoke(returnedUser))) :
                x => x.Get(It.IsAny<UserPred>());

            _userRepo.Setup(expression)
            .Returns(Task.FromResult(returnedUser))
            .Verifiable();
        }

        private void Mock_BlogRepository_Get(Blog returnedBlog)
        {
            Expression<Func<IBlogRepository, Task<Blog>>> expression =
                returnedBlog != null ?
                x => x.Get(It.Is<BlogPred>(x => x.Compile().Invoke(returnedBlog))) :
                x => x.Get(It.IsAny<BlogPred>());

            _blogRepo.Setup(expression)
            .Returns(Task.FromResult(returnedBlog))
            .Verifiable();
        }

        private void Mock_PostRepository_Get(Post returnedPost)
        {
            Expression<Func<IPostRepository, Task<Post>>> expression =
                returnedPost != null ?
                x => x.Get(It.Is<PostPred>(x => x.Compile().Invoke(returnedPost))) :
                x => x.Get(It.IsAny<PostPred>());

            _postRepo.Setup(expression)
            .Returns(Task.FromResult(returnedPost))
            .Verifiable();
        }

        private void Mock_UserPostDataRepository_GetWithPost(UserPostData userPostData)
        {
            Expression<Func<IUserPostDataRepository, Task<UserPostData>>> expression =
                userPostData != null ?
                x => x.GetWithPost(It.Is<UserPostDataPred>(x => x.Compile().Invoke(userPostData))) :
                x => x.GetWithPost(It.IsAny<UserPostDataPred>());

            _userPostDataRepo.Setup(expression)
            .Returns(Task.FromResult(userPostData))
            .Verifiable();
        }

        private void Mock_ReaderRepository_SaveAllAsync(bool returnedValue)
        {
            _readerRepo.Setup(x => x.SaveAllAsync())
                            .Returns(Task.FromResult(returnedValue))
                            .Verifiable();
        }

        private void Mock_FeedService_GetFeed(string url, string returnedValue)
        {
            _feedService.Setup(x => x.GetContent(url))
                        .Returns(Task.FromResult(returnedValue))
                        .Verifiable();
        }
        #endregion

        #region GetUserPostDataList
        [Test]
        public async Task GetUserPostDataList_CantFindUserFromClaim_Unauthorized()
        {
            //ARRANGE
            Mock_UserRepository_Get(null);

            //ACT
            var result = await _blogController.GetUserPostDataList(BLOG_ID);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status401Unauthorized));
        }

        [Test]
        public async Task GetUserPostDataList_GetList_ListWithData()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_UserPostDataRepository_GetListWithPosts(_resultList);

            //ACT
            var result = await _blogController.GetUserPostDataList(BLOG_ID);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.IsInstanceOf<IEnumerable<UserPostData>>(result.Result);
            Assert.That(result.Result, Is.EquivalentTo(_resultList));
        }
        #endregion

        #region ReadPost
        [Test]
        public async Task ReadPost_CantFindUserFromClaim_Unauthorized()
        {
            //ARRANGE
            Mock_UserRepository_Get(null);

            //ACT
            var result = await _blogController.ReadPost(0, null);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status401Unauthorized));
        }

        [Test]
        public async Task ReadPost_BlogWithIdDoesntExist_ErrEntityNotExists()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(null);

            //ACT
            var result = await _blogController.ReadPost(0, null);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(ErrEntityNotExists.StatusCode));
        }

        [Test]
        public async Task ReadPost_SaveAllAsyncFailder_ErrRequestFailed()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_ReaderRepository_SaveAllAsync(false);

            //ACT
            var result = await _blogController.ReadPost(_blog.Id, _readPostModel);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(ErrRequestFailed.StatusCode));
            _readerRepo.Verify(x => x.SaveAllAsync());
        }

        [Test]
        public async Task ReadPost_CreateNewPostAndUserPostData_NewUserPostData()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_ReaderRepository_SaveAllAsync(true);
            Mock_PostRepository_Get(null);

            //ACT
            var start_time = DateTime.UtcNow;
            var result = await _blogController.ReadPost(_blog.Id, _readPostModel);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
            Assert.IsInstanceOf<UserPostData>(result.Result);

            var result_obj = result.Result as UserPostData;
            Assert.That(result_obj.Post.Url, Is.EqualTo(_readPostModel.PostUrl));
            Assert.That(result_obj.Post.Name, Is.EqualTo(_readPostModel.Name));
            Assert.That(result_obj.Post.Blog.Id, Is.EqualTo(_blog.Id));
            Assert.That(result_obj.User.Id, Is.EqualTo(_user.Id));
            Assert.That(result_obj.FirstDateOpen, Is.GreaterThanOrEqualTo(start_time));
            Assert.That(result_obj.LastDateOpen, Is.GreaterThanOrEqualTo(start_time));

            _readerRepo.Verify(x => x.SaveAllAsync());
            _readerRepo.Verify(x => x.Add(It.IsAny<Post>()));
            _readerRepo.Verify(x => x.Add(It.IsAny<UserPostData>()));
        }

        [Test]
        public async Task ReadPost_GetPostAndCreateUserPostData_NewUserPostData()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_ReaderRepository_SaveAllAsync(true);
            Mock_PostRepository_Get(_post);
            Mock_UserPostDataRepository_GetWithPost(null);

            //ACT
            var start_time = DateTime.UtcNow;
            var result = await _blogController.ReadPost(_blog.Id, _readPostModel);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
            Assert.IsInstanceOf<UserPostData>(result.Result);

            var result_obj = result.Result as UserPostData;
            Assert.That(result_obj.Post.Url, Is.EqualTo(_readPostModel.PostUrl));
            Assert.That(result_obj.Post.Name, Is.EqualTo(_readPostModel.Name));
            Assert.That(result_obj.Post.Blog.Id, Is.EqualTo(_blog.Id));
            Assert.That(result_obj.User.Id, Is.EqualTo(_user.Id));
            Assert.That(result_obj.FirstDateOpen, Is.GreaterThanOrEqualTo(start_time));
            Assert.That(result_obj.LastDateOpen, Is.GreaterThanOrEqualTo(start_time));

            _readerRepo.Verify(x => x.SaveAllAsync());
            _readerRepo.Verify(x => x.Add(It.IsAny<Post>()), Times.Never);
            _readerRepo.Verify(x => x.Add(It.IsAny<UserPostData>()));
        }

        [Test]
        public async Task ReadPost_GetPostAndUpdateUserPostData_UpdatedUserPostData()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_ReaderRepository_SaveAllAsync(true);
            Mock_PostRepository_Get(_post);
            Mock_PostRepository_Get(_post);
            Mock_UserPostDataRepository_GetWithPost(_userPostData);

            //ACT
            var start_time = DateTime.UtcNow;
            var result = await _blogController.ReadPost(_blog.Id, _readPostModel);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.IsInstanceOf<UserPostData>(result.Result);

            var result_obj = result.Result as UserPostData;
            Assert.That(result_obj.Post.Url, Is.EqualTo(_readPostModel.PostUrl));
            Assert.That(result_obj.Post.Name, Is.EqualTo(_readPostModel.Name));
            Assert.That(result_obj.Post.Blog.Id, Is.EqualTo(_blog.Id));
            Assert.That(result_obj.User.Id, Is.EqualTo(_user.Id));
            Assert.That(result_obj.FirstDateOpen, Is.LessThanOrEqualTo(start_time));
            Assert.That(result_obj.LastDateOpen, Is.GreaterThanOrEqualTo(start_time));

            _readerRepo.Verify(x => x.SaveAllAsync());
            _readerRepo.Verify(x => x.Add(It.IsAny<Post>()), Times.Never);
            _readerRepo.Verify(x => x.Add(It.IsAny<UserPostData>()), Times.Never);
            _readerRepo.Verify(x => x.Update(It.IsAny<UserPostData>()));
        }
        #endregion

        #region Open

        [Test]
        public async Task Open_HappyPath_ReturnsPostList()
        {
            //ARRANGE
            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata.xml"))
            {
                feed_data = r.ReadToEnd();
            }
             
            Mock_BlogRepository_Get(_blog);
            Mock_FeedService_GetFeed(_blog.Url, feed_data);

            //ACT
            var result = await _blogController.Open(_blog.Id);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.IsInstanceOf<IEnumerable<RssSchema>>(result.Result);
            var list = result.Result as IEnumerable<RssSchema>;
            Assert.That(list.Count(), Is.GreaterThan(5));
        }

        [Test]
        public async Task Open_CantGetFeed_ErrExternalServerError()
        {
            //ARRANGE
            Mock_BlogRepository_Get(_blog);
            Mock_FeedService_GetFeed(_blog.Url, null);
            //ACT
            var result = await _blogController.Open(_blog.Id);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status400BadRequest));
            Assert.That(result.Message, Is.EqualTo(MsgErrExternalServerIssue));
            _feedService.Verify(x => x.GetContent(_blog.Url), Times.Once);
        }

        [Test]
        public async Task Open_CantParseFeed_ErrParsing()
        {
            //ARRANGE
            const string FAILING_FEED_DATA = "{ 123dsf234asd3454: 123}";
            Mock_BlogRepository_Get(_blog);
            Mock_FeedService_GetFeed(_blog.Url, FAILING_FEED_DATA);

            //ACT
            var result = await _blogController.Open(_blog.Id);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status400BadRequest));
            Assert.That(result.Message, Is.EqualTo(MsgErrParsing));
            _feedService.Verify(x => x.GetContent(_blog.Url), Times.Once);
            _feedService.Verify(x => x.ParseFeed(FAILING_FEED_DATA), Times.Once);
        }

        [Test]
        public async Task Open_CantFindBlog_ErrEntityNotExists()
        {
            //ARRANGE
            Mock_BlogRepository_Get(null);
            //ACT
            var result = await _blogController.Open(_blog.Id);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }
        #endregion
    }
}
