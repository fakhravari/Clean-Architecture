using Application.Features.Account.Commands.Login.Dto;
using Domain.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Account.Commands.Login
{
    public sealed class LoginCommand : IRequest<BaseResponse<LoginResponseDto>>
    {
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }
    }
}
