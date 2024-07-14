using Application.Contracts.Persistence;
using Application.Model.Personel;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class PersonelRepository : IPersonelRepository
    {
        private readonly IApplicationDbContextFactory _dbContext;
        public PersonelRepository(IApplicationDbContextFactory dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LoginDto> Login(string UserName, string Password)
        {
            using (var context = _dbContext.CreateDbContext(true))
            {
                var matches = await context.Personels.AsNoTracking().FirstOrDefaultAsync(e => e.UserName == UserName && e.Password == Password);

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
}
