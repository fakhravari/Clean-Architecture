using Application.Contracts.Persistence.IRepository;
using Application.Model.Personel;
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
    Task<GenerateJwtTokenModel> GenerateJwtToken(LoginDto Id);
    Task<bool> ValidateToken(string token);
    TokenValidationParameters TokenValidationParameters { get; }
    string X_Token_JWT { get; }


    string? IdUser { get; }
}

public class JwtService : IJwtService
{
    private readonly IPersonelRepository personelRepository;
    private readonly JwtSettingModel jwtSettings;
    public TokenValidationParameters TokenValidationParameters { get; }
    public string X_Token_JWT { get; }
    public string IdUser { get; private set; } = string.Empty;

    public JwtService(IOptionsSnapshot<JwtSettingModel> _jwtSettings, IPersonelRepository personelRepository)
    {
        this.personelRepository = personelRepository;
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

    public async Task<GenerateJwtTokenModel> GenerateJwtToken(LoginDto model)
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
                    new(ClaimTypes.NameIdentifier, model.Id.ToString()),
                    new(ClaimTypes.Name, model.Id.ToString()),
                    new(ClaimTypes.DateOfBirth, "1369/01/07"),
                    new(ClaimTypes.Actor, model.FirstName+" "+model.LastName),
                    new(ClaimTypes.Hash, model.NationalCode)
                })
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(descriptor);
        var jwt = tokenHandler.WriteToken(securityToken);

        jwt = jwt.Encrypt();

        var Ref = await personelRepository.TokenSave(jwt, model.Id);

        return new GenerateJwtTokenModel() { Token = jwt, RefreshToken = Ref.ToString(), Status = (Guid.Empty != Ref) };
    }
    public async Task<bool> ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;

        try
        {
            var NewToken = token.Replace("Bearer", string.Empty).Trim();

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(NewToken, this.TokenValidationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            IdUser = jwtToken.Claims.First(claim => claim.Type == "nameid").Value;
            var user2 = jwtToken.Claims.First(claim => claim.Type == "unique_name").Value;

            var item = await personelRepository.ValidateToken(token, long.Parse(IdUser));
            return item;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}