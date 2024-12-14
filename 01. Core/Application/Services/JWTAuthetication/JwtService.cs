using Application.Contracts.Persistence.IRepository;
using Domain.Model.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.ExtensionMethod;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.JWTAuthetication;

public interface IJwtAuthenticatedService
{
    string GenerateJwtToken(decimal id);
    Task<bool> ValidateToken(string token);
    TokenValidationParameters TokenValidationParameters { get; }

    string X_Token_JWT { get; }
    long IdUser { get; }
}

public class JwtAuthenticatedService : IJwtAuthenticatedService
{
    private readonly Lazy<IPersonelRepository> _authRepository;

    private readonly JwtSettingModel _jwtSetting;
    public TokenValidationParameters TokenValidationParameters { get; }

    public string X_Token_JWT { get; }
    public long IdUser { get; private set; } = 0;

    public JwtAuthenticatedService(IOptions<JwtSettingModel> jwtSettings, Lazy<IPersonelRepository> authRepository)
    {
        _authRepository = authRepository;

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

    public string GenerateJwtToken(decimal Id)
    {
        var secretKey = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
            SecurityAlgorithms.HmacSha256Signature);

        var encryptionkey = Encoding.UTF8.GetBytes(_jwtSetting.Encryptkey);
        var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

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
    public async Task<bool> ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;

        try
        {
            token = token.Replace("Bearer", string.Empty).Trim();

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, this.TokenValidationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            IdUser = jwtToken.Claims.First(claim => claim.Type == "nameid").Value.ToInt();

            var item = await _authRepository.Value.ValidateToken(token, IdUser);

            return item;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}