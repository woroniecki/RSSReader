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
using System.IO;
using AutoMapper;
using RSSReader.Helpers;
using RSSReader.UnitTests.Wrappers.Repositories;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class SubscriptionsControllerTests
    {
        private IMapper _mapper;
        private Mock<FeedService> _feedService;
        private Mock<IHttpService> _httpService;
        private SubscriptionController _subscriptionController;
        private SubscriptionForAddDto _subForAddDto;
        private ApiUser _user;
        private Blog _blog;
        private Subscription _subscription;
        private MockUOW _mockUOW;

        [SetUp]
        public void SetUp()
        {
            //Mocks
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

            _mockUOW = new MockUOW();
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            //Controller
            _subscriptionController = new SubscriptionController(
                _mockUOW.Object,
                _feedService.Object,
                _httpService.Object
                );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                    new Claim(ClaimTypes.NameIdentifier, _user.Id)
                               }, "TestAuthentication"));
            _subscriptionController.ControllerContext = new ControllerContext();
            _subscriptionController.ControllerContext.HttpContext = new DefaultHttpContext{ User=user };
        }

        #region Mock
        
        private void Mock_HttpService_GetStringContent(string url, string returnedValue)
        {
            _httpService.Setup(x => x.GetStringContent(url))
                            .Returns(Task.FromResult(returnedValue));
        }
        #endregion

        #region Subscribe

        [Test]
        public async Task Subscribe_CreateBlogSubAndBlogRowInDB_CreatedResult()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByUrl(_subForAddDto.BlogUrl, null);
            _mockUOW.BlogRepo.SetAddNoSave(It.IsAny<Blog>());
            _mockUOW.SubscriptionRepo.SetAddNoSave(It.IsAny<Subscription>());
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            string feed_data = null;
            using (StreamReader r = new StreamReader("../../../Data/feeddata.xml"))
            {
                feed_data = r.ReadToEnd();
            }
            Mock_HttpService_GetStringContent(_subForAddDto.BlogUrl,
                feed_data
                );

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
            Assert.That(result.Message, Is.EqualTo(MsgCreated));
            _mockUOW.BlogRepo.Verify(x => x.AddNoSave(It.IsAny<Blog>()), Times.Once);
            _mockUOW.SubscriptionRepo.Verify(x => x.AddNoSave(It.IsAny<Subscription>()), Times.Once);
            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync(), Times.Once);
        }

        [Test]
        public async Task Subscribe_CreatesBlogSubWithAlreadyExisitingBlogRow_CreatedResult()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);

            _mockUOW.BlogRepo.SetGetByUrl(_subForAddDto.BlogUrl, _blog);
            _mockUOW.SubscriptionRepo.GetByUserAndBlog(_user, _blog, null);

            _mockUOW.SubscriptionRepo.SetAddNoSave(It.IsAny<Subscription>());
            _mockUOW.ReaderRepo.SetSaveAllAsync(true);

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
            Assert.That(result.Message, Is.EqualTo(MsgCreated));
            _mockUOW.BlogRepo.Verify(x => x.AddNoSave(It.IsAny<Blog>()), Times.Never);
            _mockUOW.SubscriptionRepo.Verify(x => x.AddNoSave(It.IsAny<Subscription>()), Times.Once);
            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync(), Times.Once);
        }

        [Test]
        public async Task Subscribe_EnablesAlreadyExistingBlogsub_Ok()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _mockUOW.BlogRepo.SetGetByUrl(_subForAddDto.BlogUrl, _blog);

            _subscription.Active = false;
            _mockUOW.SubscriptionRepo.GetByUserAndBlog(_user, _blog, _subscription);

            //ACT
            var startTime = DateTime.UtcNow;
            var result = await _subscriptionController.Subscribe(_subForAddDto);
            var result_data = result.Result as Subscription;

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.That(result.Message, Is.EqualTo(MsgSucceed));
            Assert.True(result_data.Active);
            Assert.That(result_data.LastSubscribeDate, Is.GreaterThanOrEqualTo(startTime));
            _mockUOW.BlogRepo.Verify(x => x.AddNoSave(It.IsAny<Blog>()), Times.Never);
            _mockUOW.SubscriptionRepo.Verify(x => x.AddNoSave(It.IsAny<Subscription>()), Times.Never);
            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync(), Times.Once);
        }

        [Test]
        public async Task Subscribe_CantFindUserFromClaims_Unauthorized()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, null);

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Subscribe_InvalidBlogUrl_BadRequest()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _subForAddDto.BlogUrl = "http://wrongurl.com.pl";
            Mock_HttpService_GetStringContent(_subForAddDto.BlogUrl, null);

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrInvalidFeedUrl));
        }

        [Test]
        public async Task Subscribe_NoFeedContentUnderUrl_BadRequest()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);
            _subForAddDto.BlogUrl = "https://blogs.microsoft.com/";
            Mock_HttpService_GetStringContent(_subForAddDto.BlogUrl, "Wrong feed data");

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrNoContentUnderFeedUrl));
        }

        #endregion

        #region Unsubscribe

        [Test]
        public async Task Unsubscribe_DisableSubscription_Ok()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithSubscriptions(_user);

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
            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync(), Times.Once);
        }

        [Test]
        public async Task Unsubscribe_CantFindUserFromClaims_Unauthorized()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, null);

            //ACT
            var result = await _subscriptionController.Unsubscribe(0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Unsubscribe_SubIsAlreadyDisabled_ErrSubAlreadyDisabled()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithSubscriptions(_user);

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
            _mockUOW.ReaderRepo.Verify(x => x.SaveAllAsync(), Times.Never);
        }

        [Test]
        public async Task Unsubscribe_SubCantBeFoundInDB_ErrEntityNotExists()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetWithSubscriptions(_user);

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
