using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace WebApi.Infrastructure;

public static class LocalizationExtensions
{
    public static IServiceCollection AddCustomLocalization(this IServiceCollection services,
        IConfiguration configuration)
    {
        var supportedCultures = configuration.GetSection("Localization:SupportedCultures")
            .Get<List<string>>().Select(p => new CultureInfo(p)).ToArray();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(configuration["Localization:DefaultRequestCulture"]);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        return services;
    }

    public static IApplicationBuilder UseCustomLocalization(this IApplicationBuilder app)
    {
        app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>()
            .Value);

        return app;
    }
}