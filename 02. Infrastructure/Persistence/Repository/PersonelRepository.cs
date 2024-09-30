using Application.Contracts.Persistence.IRepository;
using Application.Model.Personel;
using Application.Services.Serilog;
using Domain.Entities;
using Domain.Enum;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class PersonelRepository : GenericRepository<Personel>, IPersonelRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonelRepository(IUnitOfWork iUnitOfWork, ISerilogService logger, IUnitOfWork unitOfWork) : base(iUnitOfWork, logger)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginDto> Login(string UserName, string Password)
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

            var matches = await QuerySingleAsync(query => query.Where(e => e.UserName == UserName && e.Password == Password));
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
