using Application.Contracts.Infrastructure;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DI.FileService;

public static class ServiceExtensions
{
    public static void Add_FileService(this IServiceCollection services)
    {
        services.AddScoped<IFileService, Infrastructure.FileService>();


        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
        services.AddScoped<IRedisRepository, RedisRepository>();
    }
}