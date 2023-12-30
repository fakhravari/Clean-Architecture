using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Config.Jwt.Common;

namespace WebApi.Config.Jwt
{
    public interface IJwtService
    {
        string Generate(string UserName, string Password);
    }

    public class JwtService : IJwtService
    {
        private readonly SiteSettingsJwt _siteSetting;

        public JwtService(IOptionsSnapshot<SiteSettingsJwt> settings)
        {
            _siteSetting = settings.Value;
        }

        public string Generate(string UserName, string Password)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.Encryptkey);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey),
                SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var roles = GetClaims(UserName);
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddYears(_siteSetting.JwtSettings.ExpirationYear),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(roles)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var jwt = tokenHandler.WriteToken(securityToken);


            return jwt;
        }

        private IEnumerable<Claim> GetClaims(string UserName)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, UserName),
                new(ClaimTypes.Name, UserName)
            };
            return claims;
        }
    }
}