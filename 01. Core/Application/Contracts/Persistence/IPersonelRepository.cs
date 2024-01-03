using Application.Model.Personel;
using Domain.Entities;

namespace Application.Contracts.Persistence
{
    public interface IPersonelRepository : IGenericRepositoryAsync<Personel>
    {
        Task<LoginResultDto> Login(string UserName, string Password);
    }
}