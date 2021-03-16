using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RSSReader.Controllers;
using RSSReader.Data;
using RSSReader.Data.Repositories;
using RSSReader.Dtos;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using UserPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.ApiUser, bool>>;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class SubscriptionsControllerTests
    {
        private Mock<Data.Repositories.IReaderRepository> _readerRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<ISubscriptionRepository> _subRepositoryMock;
        private IFeedService _feedService;
        private SubscriptionController _subscriptionController;
        private SubscriptionForAddDto _subForAddDto;
        private ApiUser _user;
        private Blog _blog;
        private Subscription _subscription;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _readerRepository = new Mock<Data.Repositories.IReaderRepository>();
            Mock_ReaderRepository_SaveAllAsync(true);

            _userRepository = new Mock<IUserRepository>();
            _subRepositoryMock = new Mock<ISubscriptionRepository>();
            _blogRepositoryMock = new Mock<IBlogRepository>();
            _feedService = new FeedService();

            //Dto
            _subForAddDto = new Dtos.SubscriptionForAddDto()
            {
                BlogUrl = "https://blogs.microsoft.com/feed/"
            };

            //Data
            _user = new ApiUser()
            {
                Id = "0",
                UserName = "username",
                Email = "user@mail.com",
                Subscriptions = new HashSet<Subscription>()
            };
            _blog = new Blog()
            {
                Url = "https://blogs.microsoft.com/feed/",
                Name = "https://blogs.microsoft.com/feed/"
            };
            _subscription = new Subscription(_user, _blog);


            //Controller
            _subscriptionController = new SubscriptionController(
                _userRepository.Object,
                _readerRepository.Object,
                _subRepositoryMock.Object,
                _blogRepositoryMock.Object,
                _feedService
                );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                    new Claim(ClaimTypes.NameIdentifier, _user.Id)
                               }, "TestAuthentication"));
            _subscriptionController.ControllerContext = new ControllerContext();
            _subscriptionController.ControllerContext.HttpContext = new DefaultHttpContext{ User=user };
        }

        #region Mock
        
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

        private void Mock_UserRepository_GetWithSubscriptions(ApiUser returnedUser)
        {
            Expression<Func<IUserRepository, Task<ApiUser>>> expression =
                returnedUser != null ?
                x => x.GetWithSubscriptions(It.Is<UserPred>(x => x.Compile().Invoke(returnedUser))) :
                x => x.GetWithSubscriptions(It.IsAny<UserPred>());

            _userRepository.Setup(expression)
            .Returns(Task.FromResult(returnedUser))
            .Verifiable();
        }

        private void Mock_BlogRepository_GetByUrlAsync(string blogUrl, Blog returnedValue)
        {
            _blogRepositoryMock.Setup(x => x.GetByUrlAsync(blogUrl))
                            .Returns(Task.FromResult(returnedValue));
        }

        private void Mock_BlogRepository_AddAsync(Blog blog, bool returnedValue)
        {
            _blogRepositoryMock.Setup(x => x.AddAsync(blog))
                            .Returns(Task.FromResult(returnedValue));
        }

        private void Mock_BlogRepository_AddAsync(bool returnedValue)
        {
            _blogRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Blog>()))
                            .Returns(Task.FromResult(returnedValue));
        }

        private void Mock_SubscriptionRepository_GetByUserAndBlogAsync(
            ApiUser user, Blog returnedBlog, Subscription returnedSub)
        {
            _subRepositoryMock.Setup(x => x.GetByUserAndBlogAsync(_user, returnedBlog))
                            .Returns(Task.FromResult(returnedSub));
        }

        private void Mock_SubscriptionRepository_AddAsync(Subscription sub, bool returnedValue)
        {
            _subRepositoryMock.Setup(x => x.AddAsync(sub))
                            .Returns(Task.FromResult(returnedValue));
        }

        private void Mock_SubscriptionRepository_AddAsync(bool returnedValue)
        {
            _subRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                            .Returns(Task.FromResult(returnedValue));
        }

        private void Mock_ReaderRepository_SaveAllAsync(bool returnedValue)
        {
            _readerRepository.Setup(x => x.SaveAllAsync())
                            .Returns(Task.FromResult(returnedValue));
        }
        #endregion

        #region Subscribe

        [Test]
        public async Task Subscribe_CreateBlogSubAndBlogRowInDB_CreatedResult()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);

            Mock_BlogRepository_GetByUrlAsync(_subForAddDto.BlogUrl, null);
            Mock_BlogRepository_AddAsync(true);

            Mock_SubscriptionRepository_AddAsync(true);
            
            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
            Assert.That(result.Message, Is.EqualTo(MsgCreated));
            _blogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Blog>()), Times.Once);
            _subRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Once);
        }

        [Test]
        public async Task Subscribe_CreatesBlogSubWithAlreadyExisitingBlogRow_CreatedResult()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);

            Mock_BlogRepository_GetByUrlAsync(_subForAddDto.BlogUrl, _blog);
            Mock_SubscriptionRepository_GetByUserAndBlogAsync(_user, _blog, null);

            Mock_SubscriptionRepository_AddAsync(true);

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
            Assert.That(result.Message, Is.EqualTo(MsgCreated));
            _blogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Blog>()), Times.Never);
            _subRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Once);
        }

        [Test]
        public async Task Subscribe_EnablesAlreadyExistingBlogsub_Ok()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);

            Mock_BlogRepository_GetByUrlAsync(_subForAddDto.BlogUrl, _blog);

            _subscription.Active = false;
            Mock_SubscriptionRepository_GetByUserAndBlogAsync(_user, _blog, _subscription);

            //ACT
            var startTime = DateTime.UtcNow;
            var result = await _subscriptionController.Subscribe(_subForAddDto);
            var result_data = result.Result as Subscription;

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.That(result.Message, Is.EqualTo(MsgSucceed));
            Assert.True(result_data.Active);
            Assert.That(result_data.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            _readerRepository.Verify(x => x.SaveAllAsync(), Times.Once);
            _blogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Blog>()), Times.Never);
            _subRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Never);
        }

        [Test]
        public async Task Subscribe_CantFindUserFromClaims_Unauthorized()
        {
            //ARRANGE
            Mock_UserRepository_Get(null);

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Subscribe_InvalidBlogUrl_BadRequest()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            _subForAddDto.BlogUrl = "http://wrongurl.com.pl";

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrInvalidFeedUrl));
        }

        [Test]
        public async Task Subscribe_NoFeedContentUnderUrl_BadRequest()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            _subForAddDto.BlogUrl = "https://blogs.microsoft.com/";

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrNoContentUnderFeedUrl));
        }

        [Test]
        public async Task Subscribe_BlogUrlIsNotFeed_BadRequest()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        #endregion

        #region Unsubscribe

        [Test]
        public async Task Unsubscribe_DisableSubscription_Ok()
        {
            //ARRANGE
            Mock_UserRepository_GetWithSubscriptions(_user);

            var sub = new Subscription() { Id = 0, Active = true };
            _user.Subscriptions.Add(sub);

            var startTime = DateTime.UtcNow;
            //ACT
            var result = await _subscriptionController.Unsubscribe(0);
            var result_data = result.Result as Subscription;

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.That(result.Message, Is.EqualTo(MsgSucceed));
            Assert.False(result_data.Active);
            Assert.That(result_data.LastUnsubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            _readerRepository.Verify(x => x.SaveAllAsync(), Times.Once);
        }

        [Test]
        public async Task Unsubscribe_CantFindUserFromClaims_Unauthorized()
        {
            //ARRANGE
            Mock_UserRepository_Get(null);

            //ACT
            var result = await _subscriptionController.Unsubscribe(0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Unsubscribe_SubIsAlreadyDisabled_ErrSubAlreadyDisabled()
        {
            //ARRANGE
            Mock_UserRepository_GetWithSubscriptions(_user);

            var sub = new Subscription()
            {
                Id = 0,
                Active = false,
                LastUnsubscribeDate = DateTime.MinValue
            };
            _user.Subscriptions.Add(sub);

            //ACT
            var startTime = DateTime.UtcNow;
            var result = await _subscriptionController.Unsubscribe(0);
            var result_data = result.Result as Subscription;

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrSubAlreadyDisabled));
            Assert.False(sub.Active);
            Assert.That(sub.LastUnsubscribeDate, Is.LessThan(startTime));
            _readerRepository.Verify(x => x.SaveAllAsync(), Times.Never);
        }

        [Test]
        public async Task Unsubscribe_SubCantBeFoundInDB_ErrEntityNotExists()
        {
            //ARRANGE
            Mock_UserRepository_GetWithSubscriptions(_user);

            var sub = new Subscription() { Id = 1 };
            _user.Subscriptions.Add(sub);

            //ACT
            var startTime = DateTime.UtcNow;
            var result = await _subscriptionController.Unsubscribe(0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        

        #endregion
    }
}
