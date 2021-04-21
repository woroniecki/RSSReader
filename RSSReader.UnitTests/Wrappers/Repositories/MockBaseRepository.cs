using Moq;
using RSSReader.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.UnitTests.Wrappers.Repositories
{
    class MockBaseRepository<TRepostiory, TEntity> : Mock<TRepostiory>
        where TRepostiory : class, IBaseRepository<TEntity>
        where TEntity : class
    {
        public void SetGetByID(int id, TEntity returned)
        {
            Setup(x => x.GetByID(id))
                .Returns(Task.FromResult(returned));
        }
        public void SetAdd(TEntity entity, bool returned)
        {
            Expression<Func<TRepostiory, Task<bool>>> expression =
                    entity != null ?
                    x => x.Add(entity) :
                    x => x.Add(It.IsAny<TEntity>());

            Setup(expression)
                .Returns(Task.FromResult(returned));
        }
        public void SetAddNoSave(TEntity entity)
        {
            Expression<Action<TRepostiory>> expression =
                    entity != null ?
                    x => x.AddNoSave(entity) :
                    x => x.AddNoSave(It.IsAny<TEntity>());

            Setup(expression)
                .Verifiable();
        }
        public void SetRemove(TEntity entity, bool returned)
        {
            Expression<Func<TRepostiory, Task<bool>>> expression =
                    entity != null ?
                    x => x.Remove(entity) :
                    x => x.Remove(It.IsAny<TEntity>());

            Setup(expression)
                .Returns(Task.FromResult(returned));
        }
    }
}
