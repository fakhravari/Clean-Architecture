using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Application.Services.SeriLog
{
    public static class SeriLogConfig
    {
        public static void dd(this IHostApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Override("Serilog", LogEventLevel.Error)
                .Enrich.FromLogContext().Enrich.WithClientIp().Enrich.WithClientAgent().CreateLogger();

            Log.Logger = logger;
            builder.Logging.AddSerilog(logger);
            builder.Host.UseSerilog();
        }
    }
}
