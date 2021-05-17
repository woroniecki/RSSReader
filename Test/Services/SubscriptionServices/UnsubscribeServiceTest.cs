using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ServiceLayer.SubscriptionServices;
using Tests.Helpers;

namespace Tests.Services.SubscriptionServices
{
    [TestFixture]
    class UnsubscribeServiceTest
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
        public async Task Unsubscribe_SetActiveToFalse_DisabledSubscription()
        {
            //ARRANGE
            var user = new ApiUser() { };
            _context.Add(user);

            var blog = new Blog() { };
            _context.Add(blog);

            var sub_to_unsubscribe = new Subscription(user.Id, blog);
            _context.Add(sub_to_unsubscribe);

            _context.SaveChanges();

            var start_time = DateTime.UtcNow;
            var service = new UnsubscribeService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.Unsubscribe(sub_to_unsubscribe.Id, user.Id);

            //ASSERT
            Assert.That(result.Id, Is.EqualTo(sub_to_unsubscribe.Id));
            var disabled_sub = _context
                .Subscriptions
                .Where(x => x.Id == sub_to_unsubscribe.Id)
                .First();
            Assert.False(disabled_sub.Active);
            Assert.That(disabled_sub.LastUnsubscribeDate, Is.GreaterThanOrEqualTo(start_time));
        }

        [Test]
        public async Task Unsubscribe_CantFindSubWithId_Null()
        {
            //ARRANGE
            var user = new ApiUser() { };
            _context.Add(user);

            _context.SaveChanges();

            var service = new UnsubscribeService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.Unsubscribe(0, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Unsubscribe_SubBelongsToOtherUser_Null()
        {
            //ARRANGE
            var user = new ApiUser() { };
            var user2 = new ApiUser() { };
            _context.Add(user);
            _context.Add(user2);

            var blog = new Blog() { };
            _context.Add(blog);

            var sub_to_unsubscribe = new Subscription(user2.Id, blog);
            _context.Add(sub_to_unsubscribe);

            _context.SaveChanges();

            var service = new UnsubscribeService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.Unsubscribe(sub_to_unsubscribe.Id, user.Id);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }
    }
}
