using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Application.Localization
{
    public interface ISharedViewLocalizer
    {
        string GetTranslation(string key);


        string Check_The_Input_Values { get; }
    }
    public class SharedViewLocalizer : ISharedViewLocalizer
    {
        private readonly IStringLocalizer _localizer;
        public SharedViewLocalizer(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create("Resource", "Application");
        }
        public string GetTranslation(string key)
        {
            var get = _localizer[key];
            return get.Value;
        }

        public string Check_The_Input_Values { get { return GetTranslation("Check_The_Input_Values"); } }
    }



    public static class LocalizerServiceExtension
    {
        public static void AddService_Localizer(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Localization");
            var cultures = new[] { "fa", "en", "ar" };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture(cultures[0]).AddSupportedCultures(cultures).AddSupportedUICultures(cultures)
                    .RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider(),
                    //new RouteDataRequestCultureProvider()
                };
            });
            services.AddScoped<ISharedViewLocalizer, SharedViewLocalizer>();
        }

        public static void UseCultureMiddleware(this IApplicationBuilder app)
        {
            var localizationOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
        }
    }
}