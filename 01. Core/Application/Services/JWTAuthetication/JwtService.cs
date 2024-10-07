using Domain.Model.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.ExtensionMethod;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.JWTAuthetication;

public interface IJwtService
{
    string GenerateJwtToken(long Id);
    bool ValidateToken(string token);
    TokenValidationParameters TokenValidationParameters { get; }
    string X_Token_JWT { get; }
}

public class JwtService : IJwtService
{
    private readonly JwtSettingModel jwtSettings;
    public TokenValidationParameters TokenValidationParameters { get; private set; }
    public string X_Token_JWT { get; }

    public JwtService(IOptionsSnapshot<JwtSettingModel> _jwtSettings)
    {
        jwtSettings = _jwtSettings.Value;


        X_Token_JWT = jwtSettings.X_Token_JWT;
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.Encryptkey);
        TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey),
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer
        };
    }


    public string GenerateJwtToken(long Id)
    {
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
            SecurityAlgorithms.HmacSha256Signature);

        var encryptionkey = Encoding.UTF8.GetBytes(jwtSettings.Encryptkey);
        var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(jwtSettings.NotBeforeMinutes),
            Expires = DateTime.Now.AddYears(jwtSettings.ExpirationYear),
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

        jwt = jwt.Encrypt();
        return jwt;
    }
    public bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;

        try
        {
            var NewToken = token.Replace("Bearer", string.Empty).Trim();

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(NewToken, this.TokenValidationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            var user1 = jwtToken.Claims.First(claim => claim.Type == "nameid").Value.ToString().Trim();
            var user2 = jwtToken.Claims.First(claim => claim.Type == "unique_name").Value.ToString().Trim();

            if (user1.Length <= 0 && user2.Length <= 0)
            {
                return false;
            }

            return true;
        }
        catch (Exception a)
        {
            return false;
        }
    }
}