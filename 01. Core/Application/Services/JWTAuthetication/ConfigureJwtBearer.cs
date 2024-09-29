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
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var decryptedToken = token.Decrypt();
                    context.Token = decryptedToken;

                    context.HttpContext.Items["DecryptedToken"] = decryptedToken;
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                string culture = (context.HttpContext.Request.Headers["culture"].FirstOrDefault()
                                 ?? context.HttpContext.Request.Query["culture"].FirstOrDefault()
                                 ?? "fa").Trim();

                var Token = context.HttpContext.Items["DecryptedToken"] as string;

                if (string.IsNullOrWhiteSpace(Token) == false)
                {
                    var MyJwt = context.HttpContext.RequestServices.GetRequiredService<IJwtService>();
                    bool IsFail = MyJwt.ValidateToken(Token);

                    if (IsFail == false)
                    {
                        context.Fail("error");
                    }
                }
                else
                {
                    context.Fail("error");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)ApiStatusCode.UnAuthorized;

                var localizer = context.HttpContext.RequestServices.GetRequiredService<ISharedResource>();

                var Response = new BaseResponse()
                {
                    Success = false,
                    StatusCode = context.Response.StatusCode,
                    Message = localizer.Exception + " | " + context.Response.StatusCode
                };

                var jsonResponse = JsonConvert.SerializeObject(Response, JsonSettings.Settings);
                return context.Response.WriteAsync(jsonResponse);
            }
        };
    }
}
