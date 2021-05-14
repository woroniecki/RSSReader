using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer._Commons
{
    public interface IValidatedService
    {
        IImmutableList<ValidationResult> Errors { get; }
    }
}
