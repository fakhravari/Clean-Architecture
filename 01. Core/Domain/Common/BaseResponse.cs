using Domain.Enum;
using Newtonsoft.Json;

namespace Domain.Common
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } = (int)ApiResultStatusCode.Success;
        public bool Success { get; set; } = false;
        public string Message { get; set; } = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ValidationErrors { get; set; } = new List<string>();


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object Data { get; set; }
    }
}