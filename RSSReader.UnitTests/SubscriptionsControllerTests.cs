using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using RSSReader.Controllers;
using RSSReader.Data;
using RSSReader.Dtos;
using RSSReader.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class SubscriptionsControllerTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private Mock<IReaderRepository> _readerRepository;
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<ISubRepository> _subRepositoryMock;
        private Mock<SubscriptionController> _subscriptionControllerMock;
        private SubscriptionForAddDto _subForAddDto;
        private IdentityUser _user;
        private Blog _blog;
        private Subscription _subscription;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
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
            _user = new IdentityUser()
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

        #region AddBlogSubscription

        [Test]
        public async Task AddBlogSubscription_CreateBlogSubAndBlogRowInDB_CreatedResult()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<IdentityUser>>("GetCurrentUser")
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
        public async Task AddBlogSubscription_CreatesBlogSubWithAlreadyExisitingBlogRow_CreatedResult()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<IdentityUser>>("GetCurrentUser")
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
        public async Task AddBlogSubscription_EnablesAlreadyExistingBlogsub_Ok()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<IdentityUser>>("GetCurrentUser")
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

            //ACT
            var result = await _subscriptionControllerMock.Object
                .Subscribe(_subForAddDto);
            var result_data = (result as ObjectResult).Value as Subscription;

            //ASSERT
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.True(result_data.Active);
            _readerRepository.Verify(x => x.SaveAllAsync(), Times.Once);
            _blogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Blog>()), Times.Never);
            _subRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Never);
        }

        [Test]
        public async Task AddBlogSubscription_CantFindUserFromClaims_Unauthorized()
        {
            //ARRANGE
            _subscriptionControllerMock.Protected()
                .Setup<Task<IdentityUser>>("GetCurrentUser")
                .Returns(Task.FromResult<IdentityUser>(null))
                .Verifiable();

            //ACT
            var result = await _subscriptionControllerMock.Object
                .Subscribe(_subForAddDto);

            //ASSERT
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task AddBlogSubscription_InvalidBlogUrl_BadRequest()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        [Test]
        public async Task AddBlogSubscription_BlogWithDeliveredUrlNotExists_BadRequest()
        {
            //ARRANGE

            //ACT

            //ASSERT
        }

        #endregion
    }
}
