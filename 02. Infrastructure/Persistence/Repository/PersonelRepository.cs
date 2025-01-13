using Application.Contracts.Persistence.Contexts;
using Application.Contracts.Persistence.IRepository;
using Application.Model.Personel;
using Application.Services.JWTAuthetication;
using Application.Services.Serilog;
using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Repository.Generic;
using Shared.Utils;

namespace Persistence.Repository;

public class PersonelRepository : GenericRepository<Personel>, IPersonelRepository
{
    private readonly IJwtAuthService _jwtAuth;
    private readonly IUnitOfWork<FakhravariDbContext> _unitOfWork;

    public PersonelRepository(IUnitOfWork<FakhravariDbContext> iUnitOfWork, ISerilogService logger, IJwtAuthService jwtAuth) : base(iUnitOfWork, logger)
    {
        _unitOfWork = iUnitOfWork;
        _jwtAuth = jwtAuth;
    }

    public async Task<LoginDto> Login(string UserName, string Password)
    {
        _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

        var matches =
            await QuerySingleAsync(query => query.Where(e => e.UserName == UserName && e.Password == Password));
        if (matches == null)
        {
            return new LoginDto
            {
                IsLogin = false
            };
        }

        var jwt = _jwtAuth.GenerateJwtToken(matches.Id);
        var refreshToken = await TokenSave(jwt, matches.Id);

        return new LoginDto
        {
            IsLogin = true,
            FirstName = matches.FirstName,
            Id = matches.Id,
            LastName = matches.LastName,
            NationalCode = matches.NationalCode,
            RefreshToken = refreshToken,
            Token = jwt
        };
    }

    public async Task<LoginDto> Login2(long IPersonel)
    {
        _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

        var matches = await QuerySingleAsync(query => query.Where(e => e.Id == IPersonel));
        if (matches == null)
        {
            return new LoginDto
            {
                IsLogin = false
            };
        }

        var jwt = _jwtAuth.GenerateJwtToken(matches.Id);
        var refreshToken = await TokenSave(jwt, matches.Id);

        return new LoginDto
        {
            IsLogin = true,
            FirstName = matches.FirstName,
            Id = matches.Id,
            LastName = matches.LastName,
            NationalCode = matches.NationalCode,
            RefreshToken = refreshToken,
            Token = jwt
        };
    }

    public async Task<LoginDto> ValidateRefreshToken(string Token, Guid RefreshToken)
    {
        _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

        var matches =
            await _unitOfWork.Context.Tokens.FirstOrDefaultAsync(c => c.Id == RefreshToken && c.Token1 == Token);

        if (matches == null)
        {
            return new LoginDto
            {
                IsLogin = false
            };
        }

        var item = await QuerySingleAsync(query => query.Where(e => e.Id == matches.IdPersonel));

        return new LoginDto
        {
            IsLogin = true,
            FirstName = item.FirstName,
            Id = item.Id,
            LastName = item.LastName,
            NationalCode = item.NationalCode
        };
    }

    public async Task<bool> ValidateToken(string Token, long IdPersonel)
    {
        _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

        var item = await _unitOfWork.Context.Tokens.FirstOrDefaultAsync(c => c.Token1 == Token && c.IdPersonel == IdPersonel);

        if (item == null)
            return false;
        return true;
    }

    public async Task<Guid> TokenSave(string Token, long IdPersonel)
    {
        try
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Write);

            var add = new Token
            {
                DateTime = DateTime.Now,
                IdPersonel = IdPersonel,
                IsActive = true,
                Token1 = Token,
                Idconnection = "-",
                Ip = InternetUtils.GetIp,
                Id = Guid.NewGuid()
            };
            await _unitOfWork.Context.Tokens.AddAsync(add);
            await _unitOfWork.SaveChangesAsync();

            return add.Id;
        }
        catch (Exception e)
        {
            return Guid.Empty;
        }
    }




    public async Task<decimal> RabbitMQSend(string Subject, string Body, string ToEmail)
    {
        try
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Write);

            var add = new RabbitMq()
            {
                Body = Body,
                CreateDate = DateTime.UtcNow,
                IsOk = false,
                Subject = Subject,
                ToEmail = ToEmail
            };
            await _unitOfWork.Context.RabbitMqs.AddAsync(add);
            await _unitOfWork.SaveChangesAsync();

            return add.Id;
        }
        catch (Exception e)
        {
            return 0;
        }
    }

    public async Task<bool> RabbitMQReceive()
    {
        try
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Write);

            var data = await _unitOfWork.Context.RabbitMqs.AsTracking().Where(v => v.IsOk == false).ToListAsync();

            foreach (var item in data)
            {
                item.IsOk = true;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }


}