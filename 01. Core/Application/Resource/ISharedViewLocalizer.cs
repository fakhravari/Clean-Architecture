using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Application.Resource
{
    public interface ISharedViewLocalizer
    {
        string Locale(string key);


        string Check_The_Input_Values { get; }
    }

    public class SharedResource { }



    public class SharedViewLocalizer : ISharedViewLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public SharedViewLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResource", assemblyName.Name);
        }



        public string Locale(string key)
        {
            var get = _localizer[key];

            return get.Value;
        }

        public string Check_The_Input_Values { get { return _localizer["Check_The_Input_Values"].Value; } }

    }

    public static class LocalizerServiceExtension
    {
        public static void Add_Localizer_Service(this IServiceCollection services)
        {
            services.AddControllersWithViews().AddViewLocalization();
            services.AddLocalization(options => options.ResourcesPath = "Resource");
            services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
            services.AddSingleton(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            services.AddScoped<ISharedViewLocalizer, SharedViewLocalizer>();
        }
    }
}