using Moq;
using RSSReader.Data.Repositories;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BlogPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.Blog, bool>>;

namespace RSSReader.UnitTests.Wrappers.Repositories
{
    class MockBlogRepository : MockBaseRepository<IBlogRepository, Blog>
    {
        public void SetGetByUrl(string url, Blog returned)
        {
            Setup(x => x.GetByUrl(url))
                .Returns(Task.FromResult(returned));
        }
        public void SetGetWithPosts(Blog returned)
        {
            Expression<Func<IBlogRepository, Task<Blog>>> expression =
                returned != null ?
                x => x.GetWithPosts(It.Is<BlogPred>(x => x.Compile().Invoke(returned))) :
                x => x.GetWithPosts(It.IsAny<BlogPred>());

            Setup(expression)
                .Returns(Task.FromResult(returned))
                .Verifiable();
        }
    }
}
