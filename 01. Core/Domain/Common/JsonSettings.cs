using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Domain.Common;

public static class JsonSettings
{
    public static JsonSerializerSettings Settings
    {
        get { return new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new DefaultContractResolver { NamingStrategy = new DefaultNamingStrategy() } }; }
    }
}
