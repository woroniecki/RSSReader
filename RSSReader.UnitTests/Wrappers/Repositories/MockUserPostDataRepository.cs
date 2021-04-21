using Moq;
using RSSReader.Data.Repositories;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using UserPostDataPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.UserPostData, bool>>;

namespace RSSReader.UnitTests.Wrappers.Repositories
{
    class MockUserPostDataRepository : MockBaseRepository<IUserPostDataRepository, UserPostData>
    {
        public void SetGetWithPost(UserPostData returned)
        {
            Expression<Func<IUserPostDataRepository, Task<UserPostData>>> expression =
                returned != null ?
                x => x.GetWithPost(It.Is<UserPostDataPred>(x => x.Compile().Invoke(returned))) :
                x => x.GetWithPost(It.IsAny<UserPostDataPred>());

            Setup(expression)
                .Returns(Task.FromResult(returned))
                .Verifiable();
        }
        public void SetGetListWithPosts(IEnumerable<UserPostData> returned)
        {
            Expression<Func<IUserPostDataRepository, Task<IEnumerable<UserPostData>>>> expression =
                x => x.GetListWithPosts(It.IsAny<UserPostDataPred>());

            Setup(expression)
                .Returns(Task.FromResult(returned))
                .Verifiable();
        }
    }
}
