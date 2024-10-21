using Application.Contracts.Persistence.Contexts;
using Application.Contracts.Persistence.IRepository;
using Application.Model.Personel;
using Application.Services.Serilog;
using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class PersonelRepository : GenericRepository<Personel>, IPersonelRepository
    {
        private readonly IUnitOfWork<FakhravariDbContext> _unitOfWork;

        public PersonelRepository(IUnitOfWork<FakhravariDbContext> iUnitOfWork, ISerilogService logger) : base(iUnitOfWork, logger)
        {
            _unitOfWork = iUnitOfWork;
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
                    LastName = matches.LastName,
                    NationalCode = matches.NationalCode
                };
            }
        }

        public async Task<bool> ValidateRefreshToken(string Token, Guid RefreshToken)
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

            var item = await _unitOfWork.Context.Tokens.FirstOrDefaultAsync(c => c.Id == RefreshToken && c.Token1 == Token);

            if (item == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<bool> ValidateToken(string Token, long IdPersonel)
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

            var item = await _unitOfWork.Context.Tokens.FirstOrDefaultAsync(c => c.Token1 == Token && c.IdPersonel == IdPersonel);

            if (item == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<Guid> TokenSave(string Token, long IdPersonel)
        {
            try
            {
                _unitOfWork.SetDatabaseMode(DatabaseMode.Write);

                var add = new Token() { DateTime = DateTime.Now, IdPersonel = IdPersonel, IsActive = true, Token1 = Token, Idconnection = "-", Ip = "-", Id = Guid.NewGuid()};
                await _unitOfWork.Context.Tokens.AddAsync(add);
                await _unitOfWork.SaveChangesAsync();

                return add.Id;
            }
            catch (Exception e)
            {
                return Guid.Empty;
            }
        }
    }
}
