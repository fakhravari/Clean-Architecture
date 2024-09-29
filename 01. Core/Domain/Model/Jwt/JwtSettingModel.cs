﻿namespace Domain.Model.Jwt
{
    public class JwtSettingModel
    {
        public string SecretKey { get; set; }
        public string Encryptkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationYear { get; set; }
    }
}