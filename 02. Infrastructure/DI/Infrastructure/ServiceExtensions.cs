using Application.Contracts.Infrastructure;
using Domain.Model.RabiitMQ;
using Domain.Model.Redis;
using Infrastructure;
using Infrastructure.RabbitMQR;
using Infrastructure.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DI.Infrastructure;

public static class ServiceExtensions
{
    public static void Add_FileService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileRepository, FileRepository>();

        #region RabbitMQ
        services.Configure<RabbitMQSettingModel>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IRabbitMQRepository, RabbitMQRepository>();
        services.AddHostedService<RabbitMQBackgroundService>();
        #endregion
        #region Redis
        var redisSettings = configuration.GetSection("Redis").Get<RedisSettingModel>();
        var options = new ConfigurationOptions
        {
            EndPoints = { $"{redisSettings.Host}:{redisSettings.Port}" },
            Password = redisSettings.Password,
            DefaultDatabase = redisSettings.DefaultDatabase
        };
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));
        services.AddSingleton<IRedisRepository, RedisRepository>();
        #endregion
    }
}