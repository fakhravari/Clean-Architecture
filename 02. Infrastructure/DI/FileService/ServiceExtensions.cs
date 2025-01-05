using Application.Contracts.Infrastructure;
using Domain.Model.Redis;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DI.FileService;

public static class ServiceExtensions
{
    public static void Add_FileService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, Infrastructure.FileRepository>();

        var redisSettings = configuration.GetSection("Redis").Get<RedisSettingModel>();
        var options = new ConfigurationOptions
        {
            EndPoints = { $"{redisSettings.Host}:{redisSettings.Port}" },
            User = redisSettings.User,
            Password = redisSettings.Password,
            DefaultDatabase = redisSettings.DefaultDatabase
        };
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));
        services.AddScoped<IRedisRepository, RedisRepository>();
    }
}