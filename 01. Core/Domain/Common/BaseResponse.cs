using Domain.Enum;
using Newtonsoft.Json;

namespace Domain.Common
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } = (int)ApiStatusCode.Success;
        public bool Success { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; } = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Developer { get; set; } = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ValidationErrors { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object Data { get; set; }
    }
}