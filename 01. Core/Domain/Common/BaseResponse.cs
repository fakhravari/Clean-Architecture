using Newtonsoft.Json;

namespace Domain.Common
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } = 200;
        public bool Success { get; set; } = false;
        public string Message { get; set; } = null;
        public List<string> ValidationErrors { get; set; } = new List<string>();


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    }
}