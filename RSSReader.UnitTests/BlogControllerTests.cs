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
using static RSSReader.Data.UserRepository;
using UserPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.ApiUser, bool>>;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class BlogControllerTests
    {
        const int BLOG_ID = 1;

        private BlogController _blogController;
        private Mock<IBlogRepository> _blogRepo;
        private Mock<IUserRepository> _userRepository;
        private List<UserPostData> _resultList;
        private ApiUser _user;

        [SetUp]
        public void SetUp()
        {
            //Mock
            _blogRepo = new Mock<IBlogRepository>();
            _userRepository = new Mock<IUserRepository>();

            //Data
            _resultList = Enumerable.Repeat(
                new UserPostData() {  }, 1)
                .ToList();
            _user = new ApiUser()
            {
                Id = "2",
                UserName = "username",
                Email = "user@mail.com"
            };

            //Controller
            _blogController = new BlogController(_blogRepo.Object, _userRepository.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                    new Claim(ClaimTypes.NameIdentifier, _user.Id)
                               }, "TestAuthentication"));
            _blogController.ControllerContext = new ControllerContext();
            _blogController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        #region Mock

        private void Mock_BlogRepostiory_GetUserPostDatasAsync(
            int blogId, string userId,
            IEnumerable<UserPostData> returnedList)
        {
            _blogRepo.Setup(x => x.GetUserPostDatasAsync(blogId, userId))
                .Returns(Task.FromResult(returnedList))
                .Verifiable();
        }

        private void Mock_UserRepository_Get(ApiUser returnedUser)
        {
            Expression<Func<IUserRepository, Task<ApiUser>>> expression =
                returnedUser != null ?
                x => x.Get(It.Is<UserPred>(x => x.Compile().Invoke(returnedUser))) :
                x => x.Get(It.IsAny<UserPred>());

            _userRepository.Setup(expression)
            .Returns(Task.FromResult(returnedUser))
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
            Mock_BlogRepostiory_GetUserPostDatasAsync(BLOG_ID, _user.Id, _resultList);

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
        public async Task ReadPost_CreateNewPostAndUserPostData_NewUserPostData()
        {
            //ARRANGE
            //ACT
            var result = await _blogController.ReadPost();
            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
        }
        #endregion
    }
}
