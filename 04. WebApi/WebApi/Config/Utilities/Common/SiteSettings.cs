namespace WebApi.Config.Utilities.Common
{
    public class SiteSettingsJwt
    {
        public string ElmahPath { get; set; }
        public JwtSettings JwtSettings { get; set; }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Encryptkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationYear { get; set; }
    }
}