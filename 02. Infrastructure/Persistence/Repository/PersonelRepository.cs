using Application.Contracts.Persistence;
using Application.Model.Personel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class PersonelRepository : GenericRepository<Personel>, IPersonelRepository
    {
        public PersonelRepository(FakhravariDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<LoginResultDto> Login(string UserName, string Password)
        {
            var matches = await _dbContext.Personels.AsNoTracking().FirstOrDefaultAsync(e => e.UserName == UserName && e.Password == Password);

            if (matches == null)
            {
                return new LoginResultDto()
                {
                    IsLogin = false
                };
            }
            else
            {
                return new LoginResultDto()
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
