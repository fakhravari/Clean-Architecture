using Application.Contracts.Persistence.Contexts;
using Application.Contracts.Persistence.IRepository;
using Application.Model.Personel;
using Domain.Entities;

namespace Persistence.Repository
{
    public class PersonelRepository : IPersonelRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public PersonelRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<LoginDto> Login(string UserName, string Password)
        {
            _unitOfWork.CreateContext(isReadOnly: true);
            var matches = await _unitOfWork.QuerySingleAsync<Personel>(query => query.Where(e => e.UserName == UserName && e.Password == Password));
            if (matches == null)
            {
                return new LoginDto()
                {
                    IsLogin = false
                };
            }
            else
            {
                return new LoginDto()
                {
                    IsLogin = true,
                    FirstName = matches.FirstName,
                    Id = matches.Id,
                    LastName = matches.LastName
                };
            }
        }
    }
}
