using System.Globalization;

namespace WebApi.Services.Culture
{
    public class CultureService
    {
        private readonly RequestDelegate _next;
        public static string DateTimeFormat => "yyyy-MM-dd HH:mm:ss.fff";

        public CultureService(RequestDelegate next)
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
            string formattedDate = currentDate.ToString(CultureService.DateTimeFormat);

            await _next(context);
        }
    }


    public static class CultureServiceMiddleware
    {
        public static void CultureServiceBuilder(this IApplicationBuilder app)
        {
            app.UseMiddleware<CultureService>();
        }
    }
}
