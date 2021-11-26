using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS
{
    public class QueriesBus : IQueriesBus
    {
        private readonly Func<Type, IHandleQuery> _handlersFactory;
        public QueriesBus(Func<Type, IHandleQuery> handlersFactory)
        {
            _handlersFactory = handlersFactory;
        }

        public async Task<object> Get<TQuery>(TQuery query) where TQuery : IQuery
        {
            var handler = (IHandleQuery<TQuery>)_handlersFactory(typeof(IHandleQuery<TQuery>));
            var result = await handler.Handle(query);
            return result;
        }
    }

    public interface IQueriesBus
    {
        Task<object> Get<TQuery>(TQuery query) where TQuery : IQuery;
    }
}
