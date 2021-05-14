using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace LogicLayer._GenericActions
{
    public interface IActionAsync<in TIn, TOut>
    {
        IImmutableList<ValidationResult> Errors { get; }
        bool HasErrors { get; }
        Task<TOut> ActionAsync(TIn dto);
    }
}
