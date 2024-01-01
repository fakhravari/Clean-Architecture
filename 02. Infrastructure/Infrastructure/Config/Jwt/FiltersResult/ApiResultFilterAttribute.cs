using FluentValidation;
using Infrastructure.Config.Jwt.Common;
using Infrastructure.Config.Jwt.ResultDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace Infrastructure.Config.Jwt.FiltersResult
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

                var apiResult = new ApiResult(false, ApiResultStatusCode.BadRequest, null)
                {
                    Message = string.Join(" | ", Errors)
                };
                context.Result = new JsonResult(apiResult) { StatusCode = 400 };
                context.ExceptionHandled = false;
            }
        }
    }
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is OkObjectResult okObjectResult)
            {
                var apiResult = new ApiResult<object>(true, ApiResultStatusCode.Success, okObjectResult.Value);
                context.Result = new JsonResult(apiResult) { StatusCode = okObjectResult.StatusCode };
            }
            else if (context.Result is BadRequestObjectResult badRequestObjectResult)
            {
                var message = badRequestObjectResult.Value.ToString();
                if (badRequestObjectResult.Value is SerializableError errors)
                {
                    var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct().ToList();
                    message = string.Join(" | ", errorMessages);
                }

                if (badRequestObjectResult.Value is Microsoft.AspNetCore.Mvc.ValidationProblemDetails)
                {
                    var lst = new List<string>();

                    var _error = badRequestObjectResult.Value as Microsoft.AspNetCore.Mvc.ValidationProblemDetails;
                    foreach (var trace in _error.Errors.SelectMany(p => (string[])p.Value).Distinct().ToList())
                    {
                        Match match = Regex.Match(trace, @"\$\.(\w+)");
                        if (match.Success)
                        {
                            lst.Add(match.Groups[1].Value + " The field is required.");
                        }
                    }
                    message = string.Join(" | ", lst);
                }

                var apiResult = new ApiResult(false, ApiResultStatusCode.BadRequest, message);
                context.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult.StatusCode };
            }
            else if (context.Result is OkResult okResult)
            {
                var apiResult = new ApiResult(true, ApiResultStatusCode.Success);
                context.Result = new JsonResult(apiResult) { StatusCode = okResult.StatusCode };
            }
            else if (context.Result is BadRequestResult badRequestResult)
            {
                var apiResult = new ApiResult(false, ApiResultStatusCode.BadRequest);
                context.Result = new JsonResult(apiResult) { StatusCode = badRequestResult.StatusCode };
            }
            else if (context.Result is ContentResult contentResult)
            {
                var apiResult = new ApiResult(true, ApiResultStatusCode.Success, contentResult.Content);
                context.Result = new JsonResult(apiResult) { StatusCode = contentResult.StatusCode };
            }
            else if (context.Result is NotFoundResult notFoundResult)
            {
                var apiResult = new ApiResult(false, ApiResultStatusCode.NotFound);
                context.Result = new JsonResult(apiResult) { StatusCode = notFoundResult.StatusCode };
            }
            else if (context.Result is NotFoundObjectResult notFoundObjectResult)
            {
                var apiResult = new ApiResult<object>(false, ApiResultStatusCode.NotFound, notFoundObjectResult.Value);
                context.Result = new JsonResult(apiResult) { StatusCode = notFoundObjectResult.StatusCode };
            }
            else if (context.Result is ObjectResult objectResult && objectResult.StatusCode == null && !(objectResult.Value is ApiResult))
            {
                var apiResult = new ApiResult<object>(true, ApiResultStatusCode.Success, objectResult.Value);
                context.Result = new JsonResult(apiResult) { StatusCode = objectResult.StatusCode };
            }

            base.OnResultExecuting(context);
        }
    }
}