namespace Domain.Model.Jwt
{
    public class JwtSettingModel
    {
        public string SecretKey { get; set; }
        public string Encryptkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationYear { get; set; }


        public string X_Token_JWT { get; set; }
    }

    public class GenerateJwtTokenModel
    {
        public bool Status { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}