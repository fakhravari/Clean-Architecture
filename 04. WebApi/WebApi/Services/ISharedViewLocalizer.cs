using Microsoft.Extensions.Localization;
using System.Reflection;

namespace WebApi.Services
{
    public interface ISharedViewLocalizer
    {
        string Locale(string key);
    }

    public class SharedResource { }

    public class SharedViewLocalizer : ISharedViewLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public SharedViewLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("Resources", assemblyName.Name);
        }

        public string Locale(string key)
        {
            return _localizer[key].Value;
        }
    }
}
