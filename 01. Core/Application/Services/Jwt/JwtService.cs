using Domain.Model.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Jwt
{
    public interface IJwtService
    {
        string GetToken(long Id);
    }

    public class JwtService : IJwtService
    {
        private readonly JwtModel _siteSetting;

        public JwtService(IOptionsSnapshot<JwtModel> settings)
        {
            _siteSetting = settings.Value;
        }

        public string GetToken(long Id)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(_siteSetting.Encryptkey);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.Issuer,
                Audience = _siteSetting.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSetting.NotBeforeMinutes),
                Expires = DateTime.Now.AddYears(_siteSetting.ExpirationYear),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, Id.ToString()),
                    new(ClaimTypes.Name, Id.ToString()),
                    new(ClaimTypes.DateOfBirth, "1369/01/07"),
                    new(ClaimTypes.Actor, "محمدحسین فخرآوری"),
                    new(ClaimTypes.Email, "fakhravary@gmail.com")
                })
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var jwt = tokenHandler.WriteToken(securityToken);

            return jwt;
        }
    }
}