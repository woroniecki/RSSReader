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
    class MockGroupRepository : MockBaseRepository<IGroupRepository, Group>
    {
        public void SetGetListByUser(ApiUser user, IEnumerable<Group> returned)
        {
            Setup(x => x.GetListByUser(user))
                .Returns(Task.FromResult(returned));
        }
    }
}
