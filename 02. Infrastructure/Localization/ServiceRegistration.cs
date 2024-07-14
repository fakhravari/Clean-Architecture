using Localization.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Localization;

public static class ServiceRegistration
{
    public static IServiceCollection AddService_Localizer(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddScoped<ISharedResource, SharedResource>();

        var cultures = new[] { "fa-IR", "en-US", "ar-IQ" };
        var supportedCultures = cultures.Select(c => new CultureInfo(c)).ToList();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(cultures[0]);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
            {
                var culture = context.Request.Headers["culture"].ToString();
                return new ProviderCultureResult(culture, culture);
            }));
        });

        return services;
    }
}
