using Domain.Common;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

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
                    StatusCode = (int)ApiStatusCode.BadRequest,
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
                    StatusCode = (int)ApiStatusCode.NotFound,
                    Message = "NotFound OnResultExecuting"
                };
                context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
            }
        }
    }
}
