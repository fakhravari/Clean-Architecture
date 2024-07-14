using Application.Model.Personel;

namespace Application.Contracts.Persistence
{
    public interface IPersonelRepository
    {
        Task<LoginDto> Login(string UserName, string Password);
    }
}