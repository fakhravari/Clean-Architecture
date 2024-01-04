using Domain.Common;
using Domain.Enum;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace Domain.Exception
{
    public class ValidationExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                var validationErrors = new Dictionary<string, string[]>();
                var Errors = new List<string>();

                foreach (var failure in validationException.Errors)
                {
                    var propertyName = failure.PropertyName ?? "General";
                    var errorMessages = validationErrors.ContainsKey(propertyName) ? validationErrors[propertyName].ToList() : new List<string>();
                    errorMessages.Add(failure.ErrorMessage);
                    validationErrors[propertyName] = errorMessages.ToArray();

                    Errors.Add(errorMessages.ToArray()[0]);
                }

                var Response = new BaseResponse()
                {
                    Success = false,
                    StatusCode = (int)ApiResultStatusCode.BadRequest,
                    ValidationErrors = Errors,
                    Message = "zzzzzz"
                };

                context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
                context.ExceptionHandled = false;
            }
        }
    }

    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is OkObjectResult ok)
            {
                var Response = new BaseResponse()
                {
                    Success = true,
                    StatusCode = (int)ApiResultStatusCode.Success,
                    Data = ok.Value
                };
                context.Result = new JsonResult(Response);
            }
            else if (context.Result is BadRequestObjectResult bad)
            {
                string message = bad.Value.ToString();
                var Errors = new List<string>();

                if (bad.Value is ValidationProblemDetails error)
                {
                    foreach (var trace in error.Errors.SelectMany(p => (string[])p.Value).Distinct().ToList())
                    {
                        Match match = Regex.Match(trace, @"\$\.(\w+)");
                        if (match.Success)
                        {
                            Errors.Add(match.Groups[1].Value + " The field is required.");
                        }
                    }
                }

                var Response = new BaseResponse()
                {
                    Success = false,
                    StatusCode = (int)ApiResultStatusCode.BadRequest,
                    ValidationErrors = Errors,
                    Message = message
                };

                context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
            }
            else
            {
                var Response = new BaseResponse()
                {
                    Success = false,
                    StatusCode = (int)ApiResultStatusCode.NotFound,
                    Message = "NotFound OnResultExecuting"
                };
                context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
            }
        }
    }
}
