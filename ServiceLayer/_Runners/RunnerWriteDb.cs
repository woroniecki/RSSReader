using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using LogicLayer._GenericActions;

namespace ServiceLayer._Runners
{
    public class RunnerWriteDb<TIn, TOut>
    {
        private readonly IAction<TIn, TOut> _actionClass;
        private readonly DbContext _context;
        public IImmutableList<ValidationResult> Errors => _actionClass.Errors;
        public bool HasErrors => _actionClass.HasErrors;

        public RunnerWriteDb(IAction<TIn, TOut> actionClass, DbContext context)
        {
            _context = context;
            _actionClass = actionClass;
        }

        /// <summary>
        /// Only save changes is executed as async action
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public async Task<TOut> RunActionAsync(TIn dataIn)
        {
            var result = _actionClass.Action(dataIn);
            if (!HasErrors)
                await _context.SaveChangesAsync();

            return result;
        }
    }
}
