using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer._Queries
{
    public interface IHandleQuery { }

    public interface IHandleQuery<TQuery> : IHandleQuery
    where TQuery : IQuery
    {
        Task<object> Handle(TQuery query);
    }
}
