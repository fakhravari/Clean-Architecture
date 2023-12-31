using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Config.Jwt.Common;
using WebApi.Config.Jwt.Result;

namespace WebApi.Config.Jwt.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

            var jwtSettings = builder.Configuration.GetSection(nameof(SiteSettingsJwt)).Get<SiteSettingsJwt>();
            builder.Services.Configure<SiteSettingsJwt>(builder.Configuration.GetSection(nameof(SiteSettingsJwt)));
            builder.Services.AddScoped<IJwtService, JwtService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretkey = Encoding.UTF8.GetBytes(jwtSettings.JwtSettings.SecretKey);
                var encryptionkey = Encoding.UTF8.GetBytes(jwtSettings.JwtSettings.Encryptkey);

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
                    ValidAudience = jwtSettings.JwtSettings.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.JwtSettings.Issuer
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        string token = context.HttpContext.Request.Headers["IdToken"].ToString().Trim();
                        if (!token.IsNullOrEmpty())
                        {
                            bool IsError = false;

                            var tokenHandler = new JwtSecurityTokenHandler();
                            try
                            {
                                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
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
                            {
                                context.Fail("کاربر نامعتبر است");
                            }
                        }
                        else
                        {
                            context.Fail("کاربر نامعتبر است");
                        }

                        //var userRepository = context.HttpContext.RequestServices.GetRequiredService<IPersonnel>();

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
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