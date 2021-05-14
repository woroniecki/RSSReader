using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Helpers
{
    public class ElementError
    {
        public string Name { get; set; }
        public string Reason { get; set; }
    }
    public static class ModelErrors
    {

        public static object ConvertResponse(IImmutableList<ValidationResult> Errors)
        {
            List<ElementError> errors_response = new List<ElementError>();

            foreach (var error in Errors)
            {
                if (error.MemberNames.Count() > 0)
                {
                    errors_response.Add(new ElementError()
                    {
                        Name = error.MemberNames.First(),
                        Reason = error.ErrorMessage
                    });
                }
                else
                {
                    errors_response.Add(new ElementError()
                    {
                        Name = "global",
                        Reason = error.ErrorMessage
                    });
                }
            }

            return new { ValidationErrors = errors_response };
        }
    }
}
