using Application.Contracts.Persistence.IRepository;
using Domain.Common;
using Domain.Enum;
using Domain.Model.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace Application.Services.JWTAuthetication;

public static class JwtExtensions
{
    public static void ConfigureJwtBearer(this JwtBearerOptions options, JwtSettingModel jwtSettings)
    {
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.Encryptkey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey),
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer
        };

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Headers.ContainsKey("X-Token-JWT"))
                    context.Token = context.Request.Headers["X-Token-JWT"];
                return Task.CompletedTask;
            },
            OnTokenValidated = async context =>
            {
                var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtAuthService>();
                var iAuth = context.HttpContext.RequestServices.GetRequiredService<IPersonelRepository>();
                var token = (context.HttpContext.Request.Headers["X-Token-JWT"].FirstOrDefault()
                             ?? context.HttpContext.Request.Query["X-Token-JWT"].FirstOrDefault() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(token))
                {
                    var getUser = jwtService.ValidateToken(token);
                    if (getUser == null) context.Fail("error OnTokenValidated");

                    var isFail = await iAuth.ValidateToken(getUser.Token, getUser.IdUser);
                    if (!isFail) context.Fail("error OnTokenValidated");
                }
                else
                {
                    context.Fail("error OnTokenValidated");
                }
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)ApiStatusCode.UnAuthorized;

                var response = new BaseResponse
                {
                    Message = "دسترسی غیر مجاز",
                    ValidationErrors = new List<string> { "خطا در بررسی توکن" }
                };

                var jsonResponse = JsonConvert.SerializeObject(response, JsonSettings.Settings);
                return context.Response.WriteAsync(jsonResponse);
            }
        };
    }
}