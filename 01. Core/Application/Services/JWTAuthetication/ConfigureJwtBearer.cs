using Domain.Common;
using Domain.Enum;
using Localization.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shared.ExtensionMethod;

namespace Application.Services.JWTAuthetication;

public static class JwtExtensions
{
    public static void ConfigureJwtBearer(this JwtBearerOptions options, IServiceProvider serviceProvider)
    {
        var jwtService = serviceProvider.GetRequiredService<IJwtService>();

        options.TokenValidationParameters = jwtService.TokenValidationParameters;
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string token = (context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                                ?? context.HttpContext.Request.Query["Authorization"].FirstOrDefault() ?? "").Trim();

                string xToken = (context.HttpContext.Request.Headers["X-Token-JWT"].FirstOrDefault()
                                  ?? context.HttpContext.Request.Query["X-Token-JWT"].FirstOrDefault() ?? "").Trim();

                string culture = (context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault()
                                  ?? context.HttpContext.Request.Query["Accept-Language"].FirstOrDefault()
                                  ?? context.HttpContext.Request.Headers["culture"].FirstOrDefault()
                                  ?? context.HttpContext.Request.Query["culture"].FirstOrDefault() ?? "fa-IR").Trim();

                if (string.IsNullOrWhiteSpace(token) || xToken != jwtService.X_Token_JWT)
                {
                    context.HttpContext.Items["DecryptedToken"] = "";
                    context.HttpContext.Items["X_Token"] = "";
                    context.HttpContext.Items["culture"] = culture;
                    return Task.CompletedTask;
                }

                var decryptedToken = token.Decrypt();
                context.Token = decryptedToken;

                context.HttpContext.Items["DecryptedToken"] = decryptedToken;
                context.HttpContext.Items["X_Token"] = xToken;
                context.HttpContext.Items["culture"] = culture;

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var culture = context.HttpContext.Items["culture"]?.ToString();
                var token = context.HttpContext.Items["DecryptedToken"]?.ToString();
                var xToken = context.HttpContext.Items["X_Token"]?.ToString();

                if (xToken == jwtService.X_Token_JWT && string.IsNullOrWhiteSpace(token) == false)
                {
                    var isFail = jwtService.ValidateToken(token);
                    if (isFail == false)
                    {
                        context.Fail("error OnTokenValidated");
                    }
                }
                else
                {
                    context.Fail("error OnTokenValidated");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)ApiStatusCode.UnAuthorized;

                var localizer = context.HttpContext.RequestServices.GetRequiredService<ISharedResource>();

                var response = new BaseResponse()
                {
                    Success = false,
                    StatusCode = context.Response.StatusCode,
                    Message = localizer.Exception + " | OnChallenge | " + context.Response.StatusCode
                };

                var jsonResponse = JsonConvert.SerializeObject(response, JsonSettings.Settings);
                return context.Response.WriteAsync(jsonResponse);
            }
        };
    }
}
