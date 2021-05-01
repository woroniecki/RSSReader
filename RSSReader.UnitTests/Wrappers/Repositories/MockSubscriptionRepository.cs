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
    class MockSubscriptionRepository : MockBaseRepository<ISubscriptionRepository, Subscription>
    {
        public void GetByUserAndBlog(ApiUser user, Blog blog, Subscription returned)
        {
            Setup(x => x.GetByUserAndBlog(user, blog))
                .Returns(Task.FromResult(returned));
        }
        
        public void SetGetByIdWithUserAndGroup(int id, Subscription returned)
        {
            Setup(x => x.GetByIdWithUserAndGroup(id))
                .Returns(Task.FromResult(returned));
        }
    }
}
