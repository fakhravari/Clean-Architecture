using Application.Services.JWTAuthetication;
using Domain.Model.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DI.Jwt;

public static class ServiceExtensions
{
    public static void Add_JwtIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettingModel>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtAuthService, JwtAuthService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettingModel>();
            options.ConfigureJwtBearer(jwtSettings);
        });
    }
}