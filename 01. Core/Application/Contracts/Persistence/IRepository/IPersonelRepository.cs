using Application.Model.Personel;

namespace Application.Contracts.Persistence.IRepository
{
    public interface IPersonelRepository
    {
        Task<LoginDto> Login(string UserName, string Password);
        Task<bool> ValidateRefreshToken(string Token, Guid RefreshToken);
        Task<bool> ValidateToken(string Token, long IdPersonel);
        Task<Guid> TokenSave(string Token, long IdPersonel);
    }
}