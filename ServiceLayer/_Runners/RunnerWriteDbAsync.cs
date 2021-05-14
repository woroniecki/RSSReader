using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using LogicLayer._GenericActions;

namespace ServiceLayer._Runners
{
    public class RunnerWriteDbAsync<TIn, TOut>
    {
        private readonly IActionAsync<TIn, TOut> _actionClass;
        private readonly DbContext _context;
        public IImmutableList<ValidationResult> Errors => _actionClass.Errors;
        public bool HasErrors => _actionClass.HasErrors;

        public RunnerWriteDbAsync(IActionAsync<TIn, TOut> actionClass, DbContext context)
        {
            _context = context;
            _actionClass = actionClass;
        }

        public async Task<TOut> RunActionAsync(TIn dataIn)
        {
            var result = await _actionClass.ActionAsync(dataIn).ConfigureAwait(false);
            if (!HasErrors)
                await _context.SaveChangesAsync();

            return result;
        }
    }
}
