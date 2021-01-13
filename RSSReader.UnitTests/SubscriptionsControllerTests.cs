﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
        private Mock<IReaderRepository> _readerRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<ISubscriptionRepository> _subRepositoryMock;
        private SubscriptionController _subscriptionController;
        private SubscriptionForAddDto _subForAddDto;
        private ApiUser _user;
        private Blog _blog;
        private Subscription _subscription;

        [SetUp]
        public void SetUp()
        {
            //Mocks
            _readerRepository = new Mock<IReaderRepository>();
            Mock_ReaderRepository_SaveAllAsync(true);

            _userRepository = new Mock<IUserRepository>();
            _subRepositoryMock = new Mock<ISubscriptionRepository>();
            _blogRepositoryMock = new Mock<IBlogRepository>();

            _subscriptionController = new SubscriptionController(
                _userRepository.Object,
                _readerRepository.Object,
                _subRepositoryMock.Object,
                _blogRepositoryMock.Object
                );

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
                Email = "user@mail.com",
                Subscriptions = new HashSet<Subscription>()
        };
            _blog = new Blog()
            {
                Url = "Http://blog.com",
                Name = "Http://blog.com"
            };
            _subscription = new Subscription(_user, _blog);
        }

        #region Mock
        private void Mock_UserRepository_GetCurrentUser(ApiUser returnedUser)
        {
            _userRepository.Setup(x => x.GetCurrentUser(It.IsAny<Controller>()))
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
            Mock_UserRepository_GetCurrentUser(_user);

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
            Mock_UserRepository_GetCurrentUser(_user);

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
            Mock_UserRepository_GetCurrentUser(_user);

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
            Mock_UserRepository_GetCurrentUser(null);

            //ACT
            var result = await _subscriptionController.Subscribe(_subForAddDto);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
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
            Mock_UserRepository_GetCurrentUser(_user);

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
            Mock_UserRepository_GetCurrentUser(null);

            //ACT
            var result = await _subscriptionController.Unsubscribe(0);

            //ASSERT
            Assert.That(result, Is.EqualTo(ErrUnauhtorized));
        }

        [Test]
        public async Task Unsubscribe_SubIsAlreadyDisabled_ErrSubAlreadyDisabled()
        {
            //ARRANGE
            Mock_UserRepository_GetCurrentUser(_user);

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
            Mock_UserRepository_GetCurrentUser(_user);

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
