using Domain.Enum;
using Newtonsoft.Json;

namespace Domain.Common;
public class BaseResponse
{
    public int StatusCode { get; set; } = (int)ApiStatusCode.Success;
    public bool Success { get; set; } = true;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Developer { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? ValidationErrors { get; set; }
}
public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }
}