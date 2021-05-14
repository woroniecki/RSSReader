using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using LogicLayer._GenericActions;

namespace ServiceLayer._Runners
{
    public class RunnerTransactWriteDbAsync<TIn, TPass, TOut>
        where TPass : class
        where TOut : class
    {
        private readonly IActionAsync<TIn, TPass>
            _actionPart1;
        private readonly IActionAsync<TPass, TOut>
            _actionPart2;
        private readonly DbContext _context;

        public IImmutableList<ValidationResult>
            Errors
        { get; private set; }
        public bool HasErrors => Errors.Any();

        public RunnerTransactWriteDbAsync(
            DbContext context,
            IActionAsync<TIn, TPass> actionPart1,
            IActionAsync<TPass, TOut> actionPart2)
        {
            _context = context;
            _actionPart1 = actionPart1;
            _actionPart2 = actionPart2;
        }

        public async Task<TOut> RunAction(TIn dataIn)
        {
            using (var transaction =
                _context.Database.BeginTransaction())
            {
                var passResult = await RunPart(_actionPart1, dataIn);
                if (HasErrors) 
                    return null;

                var result = await RunPart(_actionPart2, passResult);

                if (!HasErrors)
                {
                    transaction.Commit();
                }
                return result;
            }
        }

        private async Task<TPartOut> RunPart<TPartIn, TPartOut>(
            IActionAsync<TPartIn, TPartOut> bizPart,
            TPartIn dataIn)
            where TPartOut : class
        {
            var result = await bizPart.ActionAsync(dataIn);
            Errors = bizPart.Errors;
            if (!HasErrors)
            {
                _context.SaveChanges();
            }
            return result;
        }
    }
}
