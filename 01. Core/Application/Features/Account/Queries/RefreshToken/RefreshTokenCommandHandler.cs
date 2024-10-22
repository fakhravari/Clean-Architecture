using Application.Contracts.Persistence.IRepository;
using Application.Features.Account.Queries.RefreshToken.Dto;
using Application.Services.JWTAuthetication;
using Domain.Common;
using MediatR;

namespace Application.Features.Account.Queries.RefreshToken
{
    public sealed class GetListProductsQuerieHandler : IRequestHandler<RefreshTokenCommand, BaseResponse<RefreshTokenResponseDto>>
    {
        private readonly IPersonelRepository personelRepository;
        private readonly IJwtService jwt;

        public GetListProductsQuerieHandler(IPersonelRepository _personelRepository, IJwtService _jwt)
        {
            personelRepository = _personelRepository;
            jwt = _jwt;
        }

        public async Task<BaseResponse<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await personelRepository.ValidateRefreshToken(request.Token, Guid.Parse(request.RefreshToken));

            if (user.IsLogin)
            {
                var GetToken = await jwt.GenerateJwtToken(user);

                return new BaseResponse<RefreshTokenResponseDto>()
                {
                    Data = new RefreshTokenResponseDto()
                    {
                        IsLogin = GetToken.Status,
                        Token = GetToken.Token,
                        RefreshToken = GetToken.RefreshToken
                    }
                };
            }
            else
            {
                return new BaseResponse<RefreshTokenResponseDto>()
                {
                    Data = new RefreshTokenResponseDto()
                    {
                        IsLogin = false
                    }
                };
            }
        }
    }
}