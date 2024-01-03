using Domain.Common;
using Domain.Enum;
using Domain.Model.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Application.Services.Jwt
{
    public static class JwtServiceRegistration
    {
        public static void AddJwtAuthenticationService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtModel>();
            builder.Services.Configure<JwtModel>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddScoped<IJwtService, JwtService>();

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
                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey),
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        string IdToken = context.HttpContext.Request.Headers["IdToken"].ToString().Trim();
                        string IdLanguage = context.HttpContext.Request.Headers["IdLanguage"].ToString().Trim();


                        // var userRepository = context.HttpContext.RequestServices.GetRequiredService<IPersonelRepository>();

                        if (!IdToken.IsNullOrEmpty())
                        {
                            bool IsError = false;

                            var tokenHandler = new JwtSecurityTokenHandler();
                            try
                            {
                                tokenHandler.ValidateToken(IdToken, validationParameters, out SecurityToken validatedToken);
                                var jwtToken = (JwtSecurityToken)validatedToken;

                                var user1 = jwtToken.Claims.First(claim => claim.Type == "nameid").Value.ToString().Trim();
                                var user2 = jwtToken.Claims.First(claim => claim.Type == "unique_name").Value.ToString().Trim();

                                if (user1.Length <= 0 && user2.Length <= 0)
                                {
                                    IsError = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                IsError = true;
                            }

                            if (IsError)
                                context.Fail("error");
                        }
                        else
                        {
                            context.Fail("error");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        string IdToken = context.HttpContext.Request.Headers["IdToken"].ToString().Trim();
                        string IdLanguage = context.HttpContext.Request.Headers["IdLanguage"].ToString().Trim();

                        context.HandleResponse();
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)ApiResultStatusCode.UnAuthorized;

                        var Response = new BaseResponse()
                        {
                            Success = false,
                            StatusCode = context.Response.StatusCode,
                            Message = "You are not authorized! (or some other custom message)"
                        };

                        var apiResult = JsonConvert.SerializeObject(Response);
                        return context.Response.WriteAsync(apiResult);
                    }
                };
            });
        }
    }
}