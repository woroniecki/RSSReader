using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using RSSReader.Controllers;
using RSSReader.Data;
using RSSReader.Dtos;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class SubscriptionsControllerTests
    {
        private Mock<UserManager<ApiUser>> _userManagerMock;
        private Mock<IReaderRepository> _readerRepository;
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<ISubRepository> _subRepositoryMock;
        private Mock<SubscriptionController> _subscriptionControllerMock;
        private SubscriptionForAddDto _subForAddDto;
        private ApiUser _user;
        private Blog _blog;
        private Subscription _subscription;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _userManagerMock = new Mock<UserManager<ApiUser>>(
                Mock.Of<IUserStore<ApiUser>>(),
                null, null, null, null, null, null, null, null
                );

            _readerRepository = new Mock<IReaderRepository>();
            _subRepositoryMock = new Mock<ISubRepository>();
            _blogRepositoryMock = new Mock<IBlogRepository>();

            _subscriptionControllerMock = new Mock<SubscriptionController>(
                _userManagerMock.Object,
                _readerRepository.Object,
                _subRepositoryMock.Object,
                _blogRepositoryMock.Object
                );
            _subscriptionControllerMock.CallBase = true;

            //Dto
            _subForAddDto = new Dtos.SubscriptionForAddDto()
            {
                BlogUrl = "Http://blog.com"
            };

            //Data
            _user = new ApiUser()
            {
                Id = "0",
                UserName = "username",
                Email = "user@mail.com"
            };
            _blog = new Blog()
            {
                Url = "Http://blog.com",
                Name = "Http://blog.com"
            };
            _subscription = new Subscription(_user, _blog);
        }

        #region Subscribe

        [Test]
        public async Task Subscribe_CreateBlogSubAndBlogRowInDB_CreatedResult()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult(_user))
                .Verifiable();

            _blogRepositoryMock.Setup(x => x.GetByUrlAsync(_subForAddDto.BlogUrl))
                .Returns(Task.FromResult<Blog>(null));
            _blogRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Blog>()))
                .Returns(Task.FromResult(true));
            
            _subRepositoryMock.Setup(x => x.GetByUserAndBlogAsync(
                _user, It.IsAny<Blog>()))
                .Returns(Task.FromResult<Subscription>(null));
            _subRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .Returns(Task.FromResult(true));
            
            //ACT
            var result = await _subscriptionControllerMock.Object
                .Subscribe(_subForAddDto);

            //ASSERT
            Assert.IsInstanceOf<CreatedResult>(result);
            _blogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Blog>()), Times.Once);
            _subRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Once);
        }

        [Test]
        public async Task Subscribe_CreatesBlogSubWithAlreadyExisitingBlogRow_CreatedResult()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult(_user))
                .Verifiable();

            _blogRepositoryMock.Setup(x => x.GetByUrlAsync(_subForAddDto.BlogUrl))
                .Returns(Task.FromResult(_blog));

            _subRepositoryMock.Setup(x => x.GetByUserAndBlogAsync(
                _user, _blog))
                .Returns(Task.FromResult<Subscription>(null));
            _subRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .Returns(Task.FromResult(true));

            //ACT
            var result = await _subscriptionControllerMock.Object
                .Subscribe(_subForAddDto);

            //ASSERT
            Assert.IsInstanceOf<CreatedResult>(result);
            _blogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Blog>()), Times.Never);
            _subRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Once);
        }

        [Test]
        public async Task Subscribe_EnablesAlreadyExistingBlogsub_Ok()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult(_user))
                .Verifiable();

            _blogRepositoryMock.Setup(x => x.GetByUrlAsync(_subForAddDto.BlogUrl))
                .Returns(Task.FromResult(_blog));

            _subscription.Active = false;
            _subRepositoryMock.Setup(x => x.GetByUserAndBlogAsync(
                _user, _blog))
                .Returns(Task.FromResult(_subscription));

            _readerRepository.Setup(x => x.SaveAllAsync())
                .Returns(Task.FromResult(true));
            var startTime = DateTime.Now;

            //ACT
            var result = await _subscriptionControllerMock.Object
                .Subscribe(_subForAddDto);
            var result_data = (result as ObjectResult).Value as Subscription;

            //ASSERT
            Assert.IsInstanceOf<OkObjectResult>(result);
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
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult<ApiUser>(null))
                .Verifiable();

            //ACT
            var result = await _subscriptionControllerMock.Object
                .Subscribe(_subForAddDto);

            //ASSERT
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task Subscribe_InvalidBlogUrl_BadRequest()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        [Test]
        public async Task Subscribe_BlogWithDeliveredUrlNotExists_BadRequest()
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
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult<ApiUser>(_user))
                .Verifiable();

            _readerRepository.Setup(x => x.SaveAllAsync())
                .Returns(Task.FromResult(true));

            _user.Subscriptions = new HashSet<Subscription>();
            var sub = new Subscription() { Id = 0, Active = true };
            _user.Subscriptions.Add(sub);

            _subRepositoryMock.Setup(x => x.Get(0))
                .Returns(Task.FromResult(sub));

            var startTime = DateTime.Now;
            //ACT
            var result = await _subscriptionControllerMock.Object
                .Unsubscribe(0);
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
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult<ApiUser>(null))
                .Verifiable();

            //ACT
            var result = await _subscriptionControllerMock.Object
                .Unsubscribe(0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Unsubscribe_CantFindSubInUserCollection_Unauthorized()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult<ApiUser>(_user))
                .Verifiable();

            _user.Subscriptions = new HashSet<Subscription>();
            _user.Subscriptions.Add(new Subscription() { Id = 1 });

            //ACT
            var result = await _subscriptionControllerMock.Object
                .Unsubscribe(0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Unsubscribe_SubIsAlreadyDisabled_ErrSubAlreadyDisabled()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult<ApiUser>(_user))
                .Verifiable();

            _user.Subscriptions = new HashSet<Subscription>();
            var sub = new Subscription()
            {
                Id = 0,
                Active = false,
                LastUnsubscribeDate = DateTime.MinValue
            };
            _user.Subscriptions.Add(sub);

            _subRepositoryMock.Setup(x => x.Get(0))
                .Returns(Task.FromResult(sub));

            var startTime = DateTime.Now;
            //ACT
            var result = await _subscriptionControllerMock.Object
                .Unsubscribe(0);
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
            _subscriptionControllerMock.Protected()
                .Setup<Task<ApiUser>>("GetCurrentUser")
                .Returns(Task.FromResult<ApiUser>(_user))
                .Verifiable();

            _user.Subscriptions = new HashSet<Subscription>();
            var sub = new Subscription() { Id = 0 };
            _user.Subscriptions.Add(sub);

            _subRepositoryMock.Setup(x => x.Get(0))
                .Returns(Task.FromResult<Subscription>(null));

            var startTime = DateTime.Now;
            //ACT
            var result = await _subscriptionControllerMock.Object
                .Unsubscribe(0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrEntityNotExists));
        }

        #endregion
    }
}
