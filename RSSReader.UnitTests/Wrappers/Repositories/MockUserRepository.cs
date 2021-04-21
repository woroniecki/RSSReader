using Moq;
using RSSReader.Data.Repositories;
using RSSReader.Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using UserPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.ApiUser, bool>>;

namespace RSSReader.UnitTests.Wrappers.Repositories
{
    class MockUserRepository : MockBaseRepository<IUserRepository, ApiUser>
    {
        public void SetGetByID(string id, ApiUser returned)
        {
            Expression<Func<IUserRepository, Task<ApiUser>>> expression =
                    id != null ?
                    x => x.GetByID(id) :
                    x => x.GetByID(It.IsAny<string>());

            Setup(expression)
                .Returns(Task.FromResult(returned));
        }

        public void SetGetByUsername(string username, ApiUser returned)
        {
            Setup(x => x.GetByUsername(username))
                .Returns(Task.FromResult(returned));
        }

        public void SetGetByEmail(string email, ApiUser returned)
        {
            Setup(x => x.GetByEmail(email))
                .Returns(Task.FromResult(returned));
        }

        public void SetGetWithRefreshTokens(ApiUser returned)
        {
            Expression<Func<IUserRepository, Task<ApiUser>>> expression =
                returned != null ?
                x => x.GetWithRefreshTokens(It.Is<UserPred>(x => x.Compile().Invoke(returned))) :
                x => x.GetWithRefreshTokens(It.IsAny<UserPred>());

            Setup(expression)
                .Returns(Task.FromResult(returned))
                .Verifiable();
        }

        public void SetGetWithSubscriptions(ApiUser returned)
        {
            Expression<Func<IUserRepository, Task<ApiUser>>> expression =
                returned != null ?
                x => x.GetWithSubscriptions(It.Is<UserPred>(x => x.Compile().Invoke(returned))) :
                x => x.GetWithSubscriptions(It.IsAny<UserPred>());

            Setup(expression)
                .Returns(Task.FromResult(returned))
                .Verifiable();
        }
    }
}
