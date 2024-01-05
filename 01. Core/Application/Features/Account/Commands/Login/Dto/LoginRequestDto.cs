using System.ComponentModel.DataAnnotations;

namespace Application.Features.Account.Commands.Login.Dto
{
    public class LoginRequestDto
    {
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }
    }
}