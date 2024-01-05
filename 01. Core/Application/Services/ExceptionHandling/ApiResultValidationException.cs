using Application.Resource;
using Domain.Common;
using Domain.Enum;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.ExceptionHandling
{
    public class ApiResultValidationException : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var Localizer = context.HttpContext.RequestServices.GetRequiredService<ISharedViewLocalizer>();

            if (context.Exception is ValidationException validationException)
            {
                var validationErrors = new Dictionary<string, string[]>();
                var Errors = new List<string>();

                foreach (var failure in validationException.Errors)
                {
                    var propertyName = failure.PropertyName ?? "General";
                    var errorMessages = validationErrors.ContainsKey(propertyName) ? validationErrors[propertyName].ToList() : new List<string>();
                    errorMessages.Add(failure.ErrorMessage);

                    Errors.Add(Localizer.Locale(errorMessages.ToArray()[0]));
                }

                var Response = new BaseResponse()
                {
                    Success = false,
                    StatusCode = (int)ApiStatusCode.BadRequest,
                    ValidationErrors = Errors,
                    Message = Localizer.Check_The_Input_Values
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
                    Developer = Localizer.Check_The_Input_Values,
                    Message = context.Exception.Message.ToString()
                };

                context.Result = new JsonResult(Response) { StatusCode = Response.StatusCode };
                context.ExceptionHandled = false;
            }
        }
    }
}
