using Newtonsoft.Json;

namespace Application.Features.Account.Queries.RefreshToken.Dto
{
    public class RefreshTokenResponseDto
    {
        public bool IsLogin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid RefreshToken { get; set; }
    }
}