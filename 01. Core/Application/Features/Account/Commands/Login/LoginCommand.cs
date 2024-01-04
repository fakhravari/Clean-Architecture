using Application.Features.Account.Commands.Login.Dto;
using MediatR;

namespace Application.Features.Account.Commands.Login
{
    public sealed class LoginCommand : IRequest<LoginResponseDto>
    {
        public LoginRequestDto Input { get; set; }
    }
}
