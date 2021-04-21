using Moq;
using RSSReader.Data.Repositories;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.UnitTests.Wrappers.Repositories
{
    class MockPostRepository : MockBaseRepository<IPostRepository, Post>
    {
        public void SetGetLatest(int blogId, int skipAmount, int amount, IEnumerable<Post> returned)
        {
            Setup(x => x.GetLatest(blogId, skipAmount, amount))
                .Returns(Task.FromResult(returned));
        }
    }
}
