using Application.Contracts.Persistence.IRepository;
using Application.Services.JWTAuthetication;
using Domain.Model.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repository;

namespace DI.Jwt;

public static class ServiceExtensions
{
    public static void Add_JwtIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettingModel>(configuration.GetSection("JwtSettings"));

        services.AddScoped<IJwtAuthenticatedService, JwtAuthenticatedService>();
        services.AddScoped<IPersonelRepository, PersonelRepository>();
        services.AddScoped<Lazy<IPersonelRepository>>(provider =>
            new Lazy<IPersonelRepository>(() => provider.GetRequiredService<IPersonelRepository>()));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettingModel>();
            options.ConfigureJwtBearer(jwtSettings);
        });
    }
}