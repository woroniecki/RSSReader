using Moq;
using RSSReader.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.UnitTests.Wrappers.Repositories
{
    class MockReaderRepository : Mock<IReaderRepository>
    {
        public void SetSaveAllAsync(bool returned)
        {
            Setup(x => x.SaveAllAsync())
                .Returns(Task.FromResult(returned));
        }
    }
}
