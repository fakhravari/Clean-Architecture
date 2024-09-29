using Domain.Common;
using Domain.Enum;
using Localization.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace Application.ExceptionsHandler
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (FluentValidation.ValidationException ex)
            {
                await HandleValidationException(context, ex);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private static async Task HandleValidationException(HttpContext context, FluentValidation.ValidationException exception)
        {
            var localizer = context.RequestServices.GetRequiredService<ISharedResource>();

            //var rqf = context.Features.Get<IRequestCultureFeature>();
            //var culture = rqf.RequestCulture.Culture;
            //var uiCulture = rqf.RequestCulture.UICulture;
            //var translation2 = localizer.CheckTheInputValues;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            var validationErrors = exception.Errors
                .GroupBy(e => e.PropertyName ?? "General")
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var response = new BaseResponse
            {
                Success = false,
                StatusCode = (int)ApiStatusCode.BadRequest,
                ValidationErrors = validationErrors.SelectMany(kvp => kvp.Value).ToList(),
                Message = localizer.Exception
            };

            var jsonResponse = JsonConvert.SerializeObject(response, JsonSettings.Settings);
            await context.Response.WriteAsync(jsonResponse);
        }

        private static Task HandleException(HttpContext context, Exception exception)
        {
            var localizer = context.RequestServices.GetRequiredService<ISharedResource>();
            string controllerName = context.Request.RouteValues["controller"]?.ToString();
            string actionName = context.Request.RouteValues["action"]?.ToString();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new BaseResponse
            {
                Success = false,
                StatusCode = (int)ApiStatusCode.ServerError,
                Message = localizer.Exception + " | " + exception.ToString()
            };

            var jsonResponse = JsonConvert.SerializeObject(response, JsonSettings.Settings);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
