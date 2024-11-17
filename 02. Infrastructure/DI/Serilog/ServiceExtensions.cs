using Application.Services.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace DI.Serilog;

public static class ServiceExtensions
{
    public static void Add_SerilogLogging(this WebApplicationBuilder builder)
    {
        //Log.Logger = new LoggerConfiguration()
        //    .ReadFrom.Configuration(builder.Configuration)
        //    .Enrich.FromLogContext()
        //    .Enrich.WithClientIp()
        //    .CreateLogger();

        //builder.Logging.ClearProviders();
        //builder.Logging.AddSerilog();

        //builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>();

        ////Log.Logger = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341", apiKey: "LSl4dHiJExDWM2SgiyyI").CreateLogger();

        builder.Services.AddScoped<ISerilogService, SerilogService>();
    }
}
