using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.JwtSettings.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.JwtSettings.Issuer,
                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        //var userRepository = context.HttpContext.RequestServices.GetRequiredService<IPersonnel>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() == false)
                            context.Fail("This token has no claims.");

                        if (claimsIdentity.Name == "test")
                        {
                            context.Fail("User is not active.");
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)ApiResultStatusCode.UnAuthorized;

                        var apiResult =JsonConvert.SerializeObject(new ApiResult<object>(false, ApiResultStatusCode.UnAuthorized, context.ErrorDescription));

                        return context.Response.WriteAsync(apiResult);
                    }
                };
            });
        }
    }
}