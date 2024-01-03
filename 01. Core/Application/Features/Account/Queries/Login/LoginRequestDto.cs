using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Account.Queries.Login
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}