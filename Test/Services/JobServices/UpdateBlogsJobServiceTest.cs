using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Code;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ServiceLayer.JobServices;
using Tests.Helpers;
using Microsoft.Extensions.Logging;

namespace Tests.Services.JobServices
{
    [TestFixture]
    class UpdateBlogsJobServiceTest
    {
        private DataContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = InMemoryDb.CreateNewContextOptions();
            _context = new DataContext(options);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task UpdateBlogs_HappyPath_ListOfSuccesses()
        {
            //ARRANGE
            var refreshDate = DateTime.UtcNow.AddDays(-1);

            var httpHelperService = new FakeHttpHelperService();

            int amount = 111;

            for (int i = 0; i < amount; i++)
            {
                var blog = new Blog() { Url = $"www.url{i}.com", LastPostsRefreshDate = refreshDate };
                _context.Add(blog);

                httpHelperService.GetStringContentFromFile(blog.Url, $"../../../Data/feeddata{(i % 3) + 1}.xml");
            }

            _context.SaveChanges();

            var service = new UpdateBlogsJobService(
                    MapperHelper.GetNewInstance(),
                    httpHelperService.Object,
                    _context,
                    new Mock<ILogger<UpdateBlogsJobService>>().Object
                );

            //ACT
            var result = await service.UpdateBlogs();

            //ASSERT
            var updated_blogs = _context.Blogs.Include(x => x.Posts).ToList();

            foreach(var blog in updated_blogs)
            {
                Assert.That(blog.Posts.Count, Is.EqualTo(10), "Added posts amount");
            }

            Assert.That(result.Succeeded.Count(), Is.EqualTo(amount), "Succeeded amount");
        }

        [Test]
        public async Task UpdateBlogs_SomeFailsAndNoUpdate_Success()
        {
            //ARRANGE
            var refreshDate = DateTime.UtcNow.AddDays(-1);

            var httpHelperService = new FakeHttpHelperService();

            int amount = 60;

            for (int i = 0; i < amount; i++)
            {
                var blog = new Blog() { Url = $"www.url{i}.com", LastPostsRefreshDate = refreshDate };
                _context.Add(blog);

                if (i % 3 == 0)//success
                {
                    httpHelperService.GetStringContentFromFile(blog.Url, $"../../../Data/feeddata{(i % 3) + 1}.xml");
                }
                else if (i % 3 == 1)//fail
                {
                    httpHelperService.GetStringContentReturnValue(blog.Url, null);
                }
                else if (i % 3 == 2)//no update
                {
                    httpHelperService.GetStringContentFromFile(blog.Url, $"../../../Data/feeddata_empty.xml");
                }
            }

            _context.SaveChanges();

            var service = new UpdateBlogsJobService(
                    MapperHelper.GetNewInstance(),
                    httpHelperService.Object,
                    _context,
                    new Mock<ILogger<UpdateBlogsJobService>>().Object
                );

            //ACT
            var result = await service.UpdateBlogs();

            //ASSERT
            Assert.That(result.Succeeded.Count(), Is.EqualTo(amount / 3), "Succeeded amount");
            Assert.That(result.Failed.Count(), Is.EqualTo(amount / 3), "Failed amount");
            Assert.That(result.NoUpdate.Count(), Is.EqualTo(amount / 3), "NoUpdate amount");
        }
    }
}
