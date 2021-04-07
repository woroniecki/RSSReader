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
using AutoMapper;
using RSSReader.Helpers;

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
        private IMapper _mapper;
        private Mock<FeedService> _feedService;
        private Mock<IHttpService> _httpService;
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
            _httpService = new Mock<IHttpService>();
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });
            _mapper = mapper.CreateMapper();
            _feedService = new Mock<FeedService>(_httpService.Object, _mapper)
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
                _feedService.Object,
                _httpService.Object,
                _mapper);
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

        private void Mock_BlogRepository_GetWithPosts(Blog returnedBlog)
        {
            Expression<Func<IBlogRepository, Task<Blog>>> expression =
                returnedBlog != null ?
                x => x.GetWithPosts(It.Is<BlogPred>(x => x.Compile().Invoke(returnedBlog))) :
                x => x.GetWithPosts(It.IsAny<BlogPred>());

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

        private void Mock_PostRepository_GetLatest(int blogId, int skipAmount, int amount, IEnumerable<Post> returnedPosts)
        {
            Expression<Func<IPostRepository, Task<IEnumerable<Post>>>> expression =
                x => x.GetLatest(blogId, skipAmount, amount);

            _postRepo.Setup(expression)
            .Returns(Task.FromResult(returnedPosts))
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

        private void Mock_UserPostDataRepository_GetLatest(int blogId, int skipAmount, int amount, IEnumerable<Post> returnedPosts)
        {
            Expression<Func<IPostRepository, Task<IEnumerable<Post>>>> expression =
                x => x.GetLatest(blogId, skipAmount, amount);

            _postRepo.Setup(expression)
            .Returns(Task.FromResult(returnedPosts))
            .Verifiable();
        }

        private void Mock_ReaderRepository_SaveAllAsync(bool returnedValue)
        {
            _readerRepo.Setup(x => x.SaveAllAsync())
                            .Returns(Task.FromResult(returnedValue))
                            .Verifiable();
        }

        private void Mock_HttpService_GetStringContent(string url, string returnedValue)
        {
            _httpService.Setup(x => x.GetStringContent(url))
                        .Returns(Task.FromResult(returnedValue))
                        .Verifiable();
        }
        #endregion

        #region ReadPost
        [Test]
        public async Task ReadPost_CantFindUserFromClaim_Unauthorized()
        {
            //ARRANGE
            Mock_UserRepository_Get(null);

            //ACT
            var result = await _blogController.ReadPost(0, 0);

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
            var result = await _blogController.ReadPost(0, 0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(ErrEntityNotExists.StatusCode));
        }

        [Test]
        public async Task ReadPost_PostWithIdDoesntExist_ErrEntityNotExists()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_PostRepository_Get(null);

            //ACT
            var result = await _blogController.ReadPost(0, 0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(ErrEntityNotExists.StatusCode));
        }

        [Test]
        public async Task ReadPost_SaveAllAsyncFailder_ErrRequestFailed()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_PostRepository_Get(_post);
            Mock_ReaderRepository_SaveAllAsync(false);

            //ACT
            var result = await _blogController.ReadPost(_blog.Id, _post.Id);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(ErrRequestFailed.StatusCode));
            _readerRepo.Verify(x => x.SaveAllAsync());
        }

        [Test]
        public async Task ReadPost_GetPostAndCreateUserPostData_NewUserPostData()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_PostRepository_Get(_post);
            Mock_ReaderRepository_SaveAllAsync(true);
            Mock_UserPostDataRepository_GetWithPost(null);

            //ACT
            var start_time = DateTime.UtcNow;
            var result = await _blogController.ReadPost(_blog.Id, _post.Id);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.IsInstanceOf<PostDataForReturnDto>(result.Result);

            var result_obj = result.Result as PostDataForReturnDto;
            Assert.That(result_obj.Url, Is.EqualTo(_post.Url));
            Assert.That(result_obj.Name, Is.EqualTo(_post.Name));
            Assert.That(result_obj.Id, Is.EqualTo(_post.Id));

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
            var result = await _blogController.ReadPost(_blog.Id, _post.Id);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.IsInstanceOf<PostDataForReturnDto>(result.Result);

            var result_obj = result.Result as PostDataForReturnDto;
            Assert.That(result_obj.Url, Is.EqualTo(_post.Url));
            Assert.That(result_obj.Name, Is.EqualTo(_post.Name));
            Assert.That(result_obj.Id, Is.EqualTo(_post.Id));

            _readerRepo.Verify(x => x.SaveAllAsync());
            _readerRepo.Verify(x => x.Add(It.IsAny<Post>()), Times.Never);
            _readerRepo.Verify(x => x.Add(It.IsAny<UserPostData>()), Times.Never);
            _readerRepo.Verify(x => x.Update(It.IsAny<UserPostData>()));
        }
        #endregion

        #region GetPosts

        [Test]
        public async Task GetPosts_HappyPath_ListOfPostsWithUserData()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_GetWithPosts(_blog);
            List<Post> returned_posts = new List<Post>();
            for (int i = 0; i < 10; i++)
            {
                Post new_post = new Post()
                {
                    Id = i,
                    Name = i.ToString()
                };

                returned_posts.Add(new_post);
            }
            int page = 0;
            Mock_PostRepository_GetLatest(
                _blog.Id, 
                page * BlogController.POSTS_PER_CALL,
                BlogController.POSTS_PER_CALL, 
                returned_posts);

            List<UserPostData> returned_user_post_data = new List<UserPostData>();
            for (int i = 0; i < 5; i++)
            {
                UserPostData user_post_data = new UserPostData()
                {
                    Readed = true,
                    Favourite = true,
                    Post = returned_posts[i]
                };
                returned_user_post_data.Add(user_post_data);
            }
            Mock_UserPostDataRepository_GetListWithPosts(returned_user_post_data);

            //ACT
            var result = await _blogController.GetPosts(_blog.Id, page);
            IEnumerable<PostDataForReturnDto> result_list =
                (IEnumerable<PostDataForReturnDto>)result.Result;

            //ASSERT
            Assert.IsInstanceOf<IEnumerable<PostDataForReturnDto>>(result.Result);
            Assert.That(
                ((IEnumerable<PostDataForReturnDto>)result.Result).Count(),
                Is.GreaterThan(0)
                );
            foreach (var post in returned_posts)
            {
                Assert.That(
                        result_list.Count(x => x.Name == post.Name),
                        Is.EqualTo(1)
                    );
            }
            Assert.That(
                        result_list.Count(x => x.Readed),
                        Is.EqualTo(returned_user_post_data.Count())
                    );
        }

        [Test]
        public async Task GetPosts_CantFindUser_ErrUnathorized()
        {
            //ARRANGE
            Mock_UserRepository_Get(null);

            //ACT
            var result = await _blogController.GetPosts(_blog.Id, 0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status401Unauthorized));
        }

        [Test]
        public async Task GetPosts_CantFindBlog_ErrEntityNotExists()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(null);

            //ACT
            var result = await _blogController.GetPosts(_blog.Id, 0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        #endregion

        #region MarkReaded

        [Test]
        public async Task MarkReaded_HappyPath_StatusOk()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_PostRepository_Get(_post);
            Mock_UserPostDataRepository_GetWithPost(_userPostData);
            Mock_ReaderRepository_SaveAllAsync(true);

            _userPostData.Readed = false;
            _userPostData.Post = _post;

            //ACT
            var result = await _blogController.MarkReaded(_blog.Id, _post.Id, true);

            //ASSERT
            Assert.IsInstanceOf<PostDataForReturnDto>(result.Result);

            var result_obj = result.Result as PostDataForReturnDto;

            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.That(_userPostData.Readed, Is.EqualTo(true));
            Assert.That(result_obj.Readed, Is.EqualTo(true));
            Assert.That(result_obj.Name, Is.EqualTo(_post.Name));
            _readerRepo.Verify(x => x.SaveAllAsync());
        }

        [Test]
        public async Task MarkReaded_SameValueOnReadFlag_StatusOk()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_PostRepository_Get(_post);
            Mock_UserPostDataRepository_GetWithPost(_userPostData);
            Mock_ReaderRepository_SaveAllAsync(true);

            _userPostData.Readed = true;
            _userPostData.Post = _post;

            //ACT
            var result = await _blogController.MarkReaded(_blog.Id, _post.Id, true);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrNothingToUpdateInEntity));
        }

        [Test]
        public async Task MarkReaded_CantFindBlog_ErrUnauhtorized()
        {
            //ARRANGE
            Mock_UserRepository_Get(null);

            //ACT
            var result = await _blogController.MarkReaded(_blog.Id, _post.Id, true);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task MarkReaded_CantFindPost_ErrEntityNotExists()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(null);

            //ACT
            var result = await _blogController.MarkReaded(_blog.Id, _post.Id, true);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        [Test]
        public async Task MarkReaded_CantFindUserPost_ErrEntityNotExists()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            Mock_BlogRepository_Get(_blog);
            Mock_PostRepository_Get(null);

            //ACT
            var result = await _blogController.MarkReaded(_blog.Id, _post.Id, true);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        #endregion

        #region MarkFavourite

        #endregion
    }
}
