using Domain.Common;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Domain.Exception
{
    public class ApiResultException : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is OkObjectResult ok)
            {
                var Response = new BaseResponse()
                {
                    Success = true,
                    StatusCode = (int)ApiStatusCode.Success,
                    Data = ok.Value
                };
                context.Result = new JsonResult(Response);
            }
            else if (context.Result is BadRequestObjectResult bad)
            {
                var Errors = new List<string>();
                if (bad.Value is ValidationProblemDetails validationProblemDetails)
                {
                    foreach (var keyValuePair in validationProblemDetails.Errors)
                    {
                        var propertyName = keyValuePair.Key;

                        if ((new List<string>() { "command", "" }).Contains(propertyName) == false)
                        {
                            foreach (var errorMessage in keyValuePair.Value)
                            {
                                Errors.Add($"مقادیر {propertyName} را بررسی کنید");
                            }
                        }
                    }

                    Errors = Errors.Distinct().ToList();

                    var Response = new BaseResponse
                    {
                        Success = false,
                        StatusCode = (int)ApiStatusCode.BadRequest,
                        ValidationErrors = Errors,
                        Message = "مقادیر ورودی را بررسی کنید"
                    };

                    context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
                }
            }
            else
            {
                var Response = new BaseResponse()
                {
                    Success = false,
                    StatusCode = (int)ApiStatusCode.NotFound,
                    Message = "NotFound OnResultExecuting"
                };
                context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
            }
        }
    }
}
