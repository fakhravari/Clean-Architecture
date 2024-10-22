using Application.Features.Account.Queries.RefreshToken.Dto;
using Domain.Common;
using MediatR;

namespace Application.Features.Account.Queries.RefreshToken
{
    public sealed class RefreshTokenCommand : IRequest<BaseResponse<RefreshTokenResponseDto>>
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
