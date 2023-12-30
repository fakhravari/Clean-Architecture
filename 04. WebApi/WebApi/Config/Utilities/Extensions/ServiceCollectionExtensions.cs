using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApi.Config.Utilities.Common;
using WebApi.Config.Utilities.Exceptions;
using WebApi.Config.Utilities.Result;

namespace WebApi.Config.Utilities.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretkey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                var encryptionkey = Encoding.UTF8.GetBytes(jwtSettings.Encryptkey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretkey),
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);

                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        //var userRepository = context.HttpContext.RequestServices.GetRequiredService<IPersonnel>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() == false)
                            context.Fail("This token has no claims.");

                        //var user = userRepository.LoginByMobile(claimsIdentity.Claims.First(b => b.Type == "Mobile").Value);

                        // if (!user.Status) context.Fail("User is not active.");
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)ApiResultStatusCode.UnAuthorized;

                        var apiResult = Newtonsoft.Json.JsonConvert.SerializeObject(new ApiResult<object>(false, ApiResultStatusCode.UnAuthorized, context.ErrorDescription));

                        return context.Response.WriteAsync(apiResult);
                    }
                };
            });
        }
    }
}