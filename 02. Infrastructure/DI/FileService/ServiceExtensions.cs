using Application.Contracts.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DI.FileService;
public static class ServiceExtensions
{
    public static void Add_FileService(this IServiceCollection services)
    {
        services.AddScoped<IFileService, Infrastructure.FileService>();
    }
}


