using Application.Model.Personel;
using Domain.Entities;

namespace Application.Contracts.Persistence.IRepository
{
    public interface IPersonelRepository
    {
        Task<LoginDto> Login(string UserName, string Password);
    }
}