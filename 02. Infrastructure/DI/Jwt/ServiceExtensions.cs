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
        services.AddScoped<IJwtService, JwtService>();

        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            options.ConfigureJwtBearer(serviceProvider);
        });
    }
}