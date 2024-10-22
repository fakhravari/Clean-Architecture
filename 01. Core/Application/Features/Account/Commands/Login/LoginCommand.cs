using Application.Features.Account.Commands.Login.Dto;
using Domain.Common;
using MediatR;

namespace Application.Features.Account.Commands.Login
{
    public sealed class LoginCommand : IRequest<BaseResponse<LoginResponseDto>>
    {
        public LoginRequestDto Input { get; set; }
    }
}
