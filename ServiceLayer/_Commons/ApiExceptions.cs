using AutoWrapper.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer._Commons
{
    public class ElementError
    {
        public string Name { get; set; }
        public string Reason { get; set; }
    }
    public static class ApiExceptions
    {
        public static ApiException FieldValidation(string fieldName, string error, int statusCode)
        {
            return new ApiException(
                    GetErrorResponseWithSingleElem(fieldName, error),
                    statusCode
                );
        }

        public static ApiException General(string error, int statusCode)
        {
            return new ApiException(
                    GetErrorResponseWithSingleElem("global", error),
                    statusCode
                );
        }

        static object GetErrorResponseWithSingleElem(string fieldName, string error)
        {
            List<ElementError> errors_response = new List<ElementError>();

            errors_response.Add(new ElementError()
            {
                Name = fieldName,
                Reason = error
            });

            return new { ValidationErrors = errors_response };
        }
    }
}
