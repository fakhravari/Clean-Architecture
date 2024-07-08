using Application.Services.Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Application.Services.SeriLog
{
    public static class ServiceExtensions
    {
        public static void Add_SeriLogService(this IHostBuilder builder)
        {
            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>();
                    services.AddScoped<ISerilogService, SerilogService>();
                });

                webBuilder.Configure(app =>
                {
                    var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                    var logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                        .MinimumLevel.Override("Serilog", LogEventLevel.Error)
                        .Enrich.FromLogContext().Enrich.WithClientIp().Enrich.WithClientAgent().CreateLogger();

                    Log.Logger = logger;
                    app.UseSerilogRequestLogging();
                });
            });
        }

    }
}
