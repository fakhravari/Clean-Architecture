using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DI.NewtonSoft;

public static class ServiceExtensions
{
    public static void Add_NewtonsoftJsonSettings(this IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new DefaultNamingStrategy() };
        });
    }
}