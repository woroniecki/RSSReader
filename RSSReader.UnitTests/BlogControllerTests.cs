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
using RSSReader.Dtos;
using RSSReader.Data.Repositories;
using Microsoft.Toolkit.Parsers.Rss;
using AutoWrapper.Wrappers;
using System.IO;
using AutoMapper;
using RSSReader.Helpers;
using RSSReader.UnitTests.Wrappers.Repositories;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class BlogControllerTests
    {
        const int BLOG_ID = 1;

        private BlogController _blogController;
        private MockUOW _mockUOW;
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

            _mockUOW = new MockUOW();

            //Controller
            _blogController = new BlogController(
                _mockUOW.Object,
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
            _mockUOW.UserRepo.SetGetByID(_user.Id, null);

            //ACT
            var result = await _blogController.ReadPost(0, 0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status401Unauthorized));
        }

        [Test]
        public async Task ReadPost_BlogWithIdDoesntExist_ErrEntityNotExists()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(0, null);

            //ACT
            var result = await _blogController.ReadPost(0, 0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(ErrEntityNotExists.StatusCode));
        }

        [Test]
        public async Task ReadPost_PostWithIdDoesntExist_ErrEntityNotExists()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(0, _blog);
            _mockUOW.PostRepo.SetGetByID(0, null);

            //ACT
            var result = await _blogController.ReadPost(0, 0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(ErrEntityNotExists.StatusCode));
        }

        [Test]
        public async Task ReadPost_SaveAllAsyncFailder_ErrRequestFailed()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, _blog);
            _mockUOW.PostRepo.SetGetByID(_post.Id, _post);
            _mockUOW.ReaderRepo.SetSaveAllAsync(false);

            //ACT
            var result = await _blogController.ReadPost(_blog.Id, _post.Id);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(ErrRequestFailed.StatusCode));
            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync());
        }

        [Test]
        public async Task ReadPost_GetPostAndCreateUserPostData_NewUserPostData()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, _blog);
            _mockUOW.PostRepo.SetGetByID(_post.Id, _post);
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);
            _mockUOW.UserPostDataRepo.SetGetWithPost(null);

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

            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync());
            _mockUOW.ReaderRepo.Verify(x => x.Add(It.IsAny<Post>()), Times.Never);
            _mockUOW.ReaderRepo.Verify(x => x.Add(It.IsAny<UserPostData>()));
        }

        [Test]
        public async Task ReadPost_GetPostAndUpdateUserPostData_UpdatedUserPostData()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, _blog);
            _mockUOW.PostRepo.SetGetByID(_post.Id, _post);
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);
            _mockUOW.UserPostDataRepo.SetGetWithPost(_userPostData);

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

            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync());
            _mockUOW.ReaderRepo.Verify(x => x.Add(It.IsAny<Post>()), Times.Never);
            _mockUOW.ReaderRepo.Verify(x => x.Add(It.IsAny<UserPostData>()), Times.Never);
            _mockUOW.ReaderRepo.Verify(x => x.Update(It.IsAny<UserPostData>()));
        }
        #endregion

        #region GetPosts

        [Test]
        public async Task GetPosts_HappyPath_ListOfPostsWithUserData()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetWithPosts(_blog);
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
            _mockUOW.PostRepo.SetGetLatest(
                _blog.Id, 
                page * BlogController.POSTS_PER_CALL,
                BlogController.POSTS_PER_CALL, 
                returned_posts
                );

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
            _mockUOW.UserPostDataRepo.SetGetListWithPosts(returned_user_post_data);

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
            _mockUOW.UserRepo.SetGetByID(_user.Id, null);

            //ACT
            var result = await _blogController.GetPosts(_blog.Id, 0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status401Unauthorized));
        }

        [Test]
        public async Task GetPosts_CantFindBlog_ErrEntityNotExists()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, null);

            //ACT
            var result = await _blogController.GetPosts(_blog.Id, 0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        #endregion

        #region MarkReaded

        [Test]
        public async Task UpdateUserPostData_HappyPathUpdateReaded_StatusOk()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, _blog);
            _mockUOW.PostRepo.SetGetByID(_post.Id, _post);
            _mockUOW.UserPostDataRepo.SetGetWithPost(_userPostData);
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            _userPostData.Readed = false;
            _userPostData.Post = _post;

            UpdateUserPostDataDto data = new UpdateUserPostDataDto()
            {
                Readed = true
            };

            //ACT
            var result = await _blogController.UpdateUserPostData(_blog.Id, _post.Id, data);

            //ASSERT
            Assert.IsInstanceOf<PostDataForReturnDto>(result.Result);

            var result_obj = result.Result as PostDataForReturnDto;

            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.That(_userPostData.Readed, Is.EqualTo(true));
            Assert.That(result_obj.Readed, Is.EqualTo(true));
            Assert.That(result_obj.Name, Is.EqualTo(_post.Name));
            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync());
        }

        [Test]
        public async Task UpdateUserPostData_SameValueOnReadFlag_ErrNothingToUpdateInEntity()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, _blog);
            _mockUOW.PostRepo.SetGetByID(_post.Id, _post);
            _mockUOW.UserPostDataRepo.SetGetWithPost(_userPostData);
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            _userPostData.Readed = true;
            _userPostData.Post = _post;
            UpdateUserPostDataDto data = new UpdateUserPostDataDto()
            {
                Readed = _userPostData.Readed
            };

            //ACT
            var result = await _blogController.UpdateUserPostData(_blog.Id, _post.Id, data);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrNothingToUpdateInEntity));
        }

        [Test]
        public async Task UpdateUserPostData_HappyPathUpdateErrNothingToUpdateInEntity_StatusOk()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, _blog);
            _mockUOW.PostRepo.SetGetByID(_post.Id, _post);
            _mockUOW.UserPostDataRepo.SetGetWithPost(_userPostData);
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            _userPostData.Favourite = false;
            _userPostData.Post = _post;

            UpdateUserPostDataDto data = new UpdateUserPostDataDto()
            {
                Favourite = true
            };

            //ACT
            var result = await _blogController.UpdateUserPostData(_blog.Id, _post.Id, data);

            //ASSERT
            Assert.IsInstanceOf<PostDataForReturnDto>(result.Result);

            var result_obj = result.Result as PostDataForReturnDto;

            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.That(_userPostData.Favourite, Is.EqualTo(true));
            Assert.That(result_obj.Favourite, Is.EqualTo(true));
            Assert.That(result_obj.Name, Is.EqualTo(_post.Name));
            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync());
        }

        [Test]
        public async Task UpdateUserPostData_SameValueOnFavouriteFlag_ErrNothingToUpdateInEntity()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, _blog);
            _mockUOW.PostRepo.SetGetByID(_post.Id, _post);
            _mockUOW.UserPostDataRepo.SetGetWithPost(_userPostData);
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            _userPostData.Favourite = true;
            _userPostData.Post = _post;
            UpdateUserPostDataDto data = new UpdateUserPostDataDto()
            {
                Favourite = _userPostData.Favourite
            };

            //ACT
            var result = await _blogController.UpdateUserPostData(_blog.Id, _post.Id, data);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrNothingToUpdateInEntity));
        }

        [Test]
        public async Task UpdateUserPostData_CantFindBlog_ErrUnauhtorized()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, null);
            UpdateUserPostDataDto data = new UpdateUserPostDataDto();

            //ACT
            var result = await _blogController.UpdateUserPostData(_blog.Id, _post.Id, data);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task UpdateUserPostData_CantFindPost_ErrEntityNotExists()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, null);
            UpdateUserPostDataDto data = new UpdateUserPostDataDto();

            //ACT
            var result = await _blogController.UpdateUserPostData(_blog.Id, _post.Id, data);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        [Test]
        public async Task UpdateUserPostData_CantFindUserPost_ErrEntityNotExists()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByID(_blog.Id, _blog);
            _mockUOW.PostRepo.SetGetByID(_post.Id, null);

            UpdateUserPostDataDto data = new UpdateUserPostDataDto();

            //ACT
            var result = await _blogController.UpdateUserPostData(_blog.Id, _post.Id, data);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        #endregion

        #region MarkFavourite

        #endregion
    }
}
