using Newtonsoft.Json;

namespace Application.Features.Account.Commands.Login.Dto
{
    public class LoginResponseDto
    {
        public bool IsLogin { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }
    }
}