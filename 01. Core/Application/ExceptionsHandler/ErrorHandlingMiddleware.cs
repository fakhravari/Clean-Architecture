using Application.Localization;
using Azure.Core;
using Domain.Common;
using Domain.Enum;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            catch (ValidationException ex)
            {
                await HandleValidationException(context, ex);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }


        private static JsonSerializerSettings JsonSettings
        {
            get { return new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new DefaultContractResolver { NamingStrategy = new DefaultNamingStrategy() } }; }
        }

        private static async Task HandleValidationException(HttpContext context, ValidationException exception)
        {
            var localizer = context.RequestServices.GetRequiredService<ISharedViewLocalizer>();



            var rqf = context.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture;
            var uiCulture = rqf.RequestCulture.UICulture;
            var translation2 = localizer.Check_The_Input_Values;







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
                Message = localizer.Check_The_Input_Values
            };

            var jsonResponse = JsonConvert.SerializeObject(response, JsonSettings);
            await context.Response.WriteAsync(jsonResponse);
        }

        private static Task HandleException(HttpContext context, Exception exception)
        {
            string controllerName = context.Request.RouteValues["controller"]?.ToString();
            string actionName = context.Request.RouteValues["action"]?.ToString();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                error = "An unexpected error occurred."
            };

            var jsonResponse = JsonConvert.SerializeObject(response, JsonSettings);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
