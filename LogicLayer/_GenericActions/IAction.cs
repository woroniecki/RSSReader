using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer._GenericActions
{
    public interface IAction<in TIn, out TOut>
    {
        IImmutableList<ValidationResult> Errors { get; }
        bool HasErrors { get; }
        TOut Action(TIn dto);
    }
}
