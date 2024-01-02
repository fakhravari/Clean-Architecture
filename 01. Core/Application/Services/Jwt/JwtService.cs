using Application.Services.Jwt.Common;
using Microsoft.Extensions.Options;

namespace Application.Services.Jwt
{
    public interface IJwtService
    {
       // string JwtTokenGenerate(LoginCommand model);
    }

    public class JwtService : IJwtService
    {
        private readonly JwtSettingsDTO _siteSetting;

        public JwtService(IOptionsSnapshot<JwtSettingsDTO> settings)
        {
            _siteSetting = settings.Value;
        }

        //public string JwtTokenGenerate(LoginCommand model)
        //{
        //    var secretKey = Encoding.UTF8.GetBytes(_siteSetting.SecretKey);
        //    var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
        //        SecurityAlgorithms.HmacSha256Signature);

        //    var encryptionkey = Encoding.UTF8.GetBytes(_siteSetting.Encryptkey);
        //    var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

        //    var descriptor = new SecurityTokenDescriptor
        //    {
        //        Issuer = _siteSetting.Issuer,
        //        Audience = _siteSetting.Audience,
        //        IssuedAt = DateTime.Now,
        //        NotBefore = DateTime.Now.AddMinutes(_siteSetting.NotBeforeMinutes),
        //        Expires = DateTime.Now.AddYears(_siteSetting.ExpirationYear),
        //        SigningCredentials = signingCredentials,
        //        EncryptingCredentials = encryptingCredentials,
        //        Subject = new ClaimsIdentity(new List<Claim>
        //        {
        //            new(ClaimTypes.NameIdentifier, model.UserName),
        //            new(ClaimTypes.Name, model.UserName),
        //            new(ClaimTypes.DateOfBirth, "1369/01/07"),
        //            new(ClaimTypes.Actor, "محمدحسین فخرآوری"),
        //            new(ClaimTypes.Email, "fakhravary@gmail.com")
        //        })
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var securityToken = tokenHandler.CreateToken(descriptor);
        //    var jwt = tokenHandler.WriteToken(securityToken);

        //    return jwt;
        //}
    }
}