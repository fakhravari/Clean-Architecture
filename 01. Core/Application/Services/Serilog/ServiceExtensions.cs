using Application.Services.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Application.Services.SeriLog
{
    public static class ServiceExtensions
    {
        public static void Add_SerilogLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Error)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithClientIp()
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>();
            builder.Services.AddScoped<ISerilogService, SerilogService>();
        }
    }
}
