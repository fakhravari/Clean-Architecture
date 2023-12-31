using Infrastructure.Config.Jwt.Common;
using Infrastructure.Config.Jwt.ResultDTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure.Config.Jwt.ServiceExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettingsDTO>();
            builder.Services.Configure<JwtSettingsDTO>(builder.Configuration.GetSection("JwtSettings"));
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

                        //var userRepository = context.HttpContext.RequestServices.GetRequiredService<IPersonnel>();

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        string IdToken = context.HttpContext.Request.Headers["IdToken"].ToString().Trim();
                        string IdLanguage = context.HttpContext.Request.Headers["IdLanguage"].ToString().Trim();

                        context.HandleResponse();
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)ApiResultStatusCode.UnAuthorized;

                        var apiResult = JsonConvert.SerializeObject(new ApiResult<object>(false, ApiResultStatusCode.UnAuthorized, context.ErrorDescription));

                        return context.Response.WriteAsync(apiResult);
                    }
                };
            });
        }
    }
}