using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Subscriptions;
using Moq;
using NUnit.Framework;
using ServiceLayer.SubscriptionServices;
using Tests.Helpers;

namespace Tests.Services.SubscriptionServices
{
    [TestFixture]
    class UpdateSubscriptionServiceTest
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
        public async Task Update_FromUnfilterToFilter_SubDto()
        {
            //ARRANGE
            var dto = new UpdateSubscriptionRequestDto() { FilterReaded = true };

            var blog = new Blog();
            _context.Add(blog);

            var user = new ApiUser();
            _context.Add(user);

            var sub = new Subscription(user.Id, blog);
            _context.Add(sub);

            _context.SaveChanges();

            var service = new UpdateSubscriptionService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.Update(sub.Id, user.Id, dto);

            //ASSERT
            Assert.That(result.FilterReaded, Is.EqualTo(dto.FilterReaded));
            var sub_result = _context.Subscriptions.FirstOrDefault(x => x.Id == sub.Id);
            Assert.That(sub_result.FilterReaded, Is.EqualTo(dto.FilterReaded));
        }

        [Test]
        public async Task Update_FromFilteredToUnfilter_SubDto()
        {
            //ARRANGE
            var dto = new UpdateSubscriptionRequestDto() { FilterReaded = false };

            var blog = new Blog();
            _context.Add(blog);

            var user = new ApiUser();
            _context.Add(user);

            var sub = new Subscription(user.Id, blog) { FilterReaded = true };
            _context.Add(sub);

            _context.SaveChanges();

            var service = new UpdateSubscriptionService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.Update(sub.Id, user.Id, dto);

            //ASSERT
            Assert.That(result.FilterReaded, Is.EqualTo(dto.FilterReaded));
            var sub_result = _context.Subscriptions.FirstOrDefault(x => x.Id == sub.Id);
            Assert.That(sub_result.FilterReaded, Is.EqualTo(dto.FilterReaded));
        }

        [Test]
        public async Task Update_SameValues_SubDto()
        {
            //ARRANGE
            var dto = new UpdateSubscriptionRequestDto() { FilterReaded = true };

            var blog = new Blog();
            _context.Add(blog);

            var user = new ApiUser();
            _context.Add(user);

            var sub = new Subscription(user.Id, blog) { FilterReaded = true };
            _context.Add(sub);

            _context.SaveChanges();

            var service = new UpdateSubscriptionService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.Update(sub.Id, user.Id, dto);

            //ASSERT
            Assert.That(result.FilterReaded, Is.EqualTo(dto.FilterReaded));
            var sub_result = _context.Subscriptions.FirstOrDefault(x => x.Id == sub.Id);
            Assert.That(sub_result.FilterReaded, Is.EqualTo(dto.FilterReaded));
        }

        [Test]
        public async Task Update_CantFindUser_Null()
        {
            //ARRANGE
            var dto = new UpdateSubscriptionRequestDto() { FilterReaded = true };

            var blog = new Blog();
            _context.Add(blog);

            var user = new ApiUser();
            _context.Add(user);

            var sub = new Subscription(user.Id, blog) { FilterReaded = true };
            _context.Add(sub);

            _context.SaveChanges();

            var service = new UpdateSubscriptionService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.Update(sub.Id, "0", dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Update_CantFindSub_Null()
        {
            //ARRANGE
            var dto = new UpdateSubscriptionRequestDto() { FilterReaded = true };

            var blog = new Blog();
            _context.Add(blog);

            var user = new ApiUser();
            _context.Add(user);

            _context.SaveChanges();

            var service = new UpdateSubscriptionService(MapperHelper.GetNewInstance(), _unitOfWork);
            //ACT

            var result = await service.Update(0, user.Id, dto);

            //ASSERT
            Assert.IsNull(result);
            Assert.That(service.Errors.Count, Is.EqualTo(1));
        }
    }
}
