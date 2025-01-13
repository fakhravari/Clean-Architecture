using Domain.Model.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.ExtensionMethod;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.JWTAuthetication;

public interface IJwtAuthService
{
    TokenValidationParameters TokenValidationParameters { get; }

    string X_Token_JWT { get; }
    long IdUser { get; }
    string GenerateJwtToken(decimal id);
    ValidateTokenModel? ValidateToken(string token);
}

public class JwtAuthService : IJwtAuthService
{
    private readonly JwtSettingModel _jwtSetting;

    public JwtAuthService(IOptions<JwtSettingModel> jwtSettings)
    {
        _jwtSetting = jwtSettings.Value;
        X_Token_JWT = jwtSettings.Value.X_Token_JWT;

        var secretKey = Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey);
        var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.Value.Encryptkey);
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
            ValidAudience = jwtSettings.Value.Audience,
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Value.Issuer
        };
    }

    public TokenValidationParameters TokenValidationParameters { get; }

    public string X_Token_JWT { get; }
    public long IdUser { get; private set; }

    public string GenerateJwtToken(decimal Id)
    {
        var secretKey = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
            SecurityAlgorithms.HmacSha256Signature);

        var encryptionkey = Encoding.UTF8.GetBytes(_jwtSetting.Encryptkey);
        var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey),
            SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

        var Claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Id.ToString()),
            new(ClaimTypes.Name, Id.ToString())
        };
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSetting.Issuer,
            Audience = _jwtSetting.Audience,
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(_jwtSetting.NotBeforeMinutes),
            Expires = DateTime.Now.AddYears(_jwtSetting.ExpirationYear),
            SigningCredentials = signingCredentials,
            EncryptingCredentials = encryptingCredentials,
            Subject = new ClaimsIdentity(Claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(descriptor);
        var jwt = tokenHandler.WriteToken(securityToken);

        return jwt;
    }

    public ValidateTokenModel? ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        try
        {
            token = token.Replace("Bearer", string.Empty).Trim();

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, TokenValidationParameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            IdUser = jwtToken.Claims.First(claim => claim.Type == "nameid").Value.ToInt();

            return new ValidateTokenModel()
            {
                Token = token,
                IdUser = IdUser
            };
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}