using Domain.Common;
using Domain.Enum;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Exception
{
    public class ApiResultValidationException : ExceptionFilterAttribute
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
                    StatusCode = (int)ApiStatusCode.BadRequest,
                    ValidationErrors = Errors,
                    Message = "مقادیر ورودی را بررسی کنید"
                };

                context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
                context.ExceptionHandled = false;
            }
            else
            {
                var Response = new BaseResponse()
                {
                    Success = false,
                    StatusCode = (int)ApiStatusCode.BadRequest,
                    Message = context.Exception.Message.ToString()
                };

                context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
                context.ExceptionHandled = false;
            }
        }
    }
}
