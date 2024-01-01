using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Shared.Resources
{
    public class ResourcesCulture
    {
        private readonly RequestDelegate _next;
        public static string DateTimeFormat => "yyyy-MM-dd HH:mm:ss";

        public ResourcesCulture(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            int cultureHeader = int.Parse(context.Request.Headers["IdLanguage"].ToString());
            string culture = string.Empty;
            switch (cultureHeader)
            {
                case 1:
                    culture = "fa-IR";
                    break;
                case 2:
                    culture = "en-US";
                    break;
                case 3:
                    culture = "ar-SA";
                    break;
                default:
                    culture = "fa-IR";
                    break;
            }

            var cultureInfo = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString(ResourcesCulture.DateTimeFormat);

            await _next(context);
        }
    }

    public static class ResourcesMiddleware
    {
        public static void ResourcesBuilder(this IApplicationBuilder app)
        {
            app.UseMiddleware<ResourcesCulture>();
        }
    }
}
