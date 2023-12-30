using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WebApi.ActionFilters;

public class SwaggerJsonIgnore : IOperationFilter
{
    //from query
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var ignoredProperties = context.MethodInfo.GetParameters()
        .SelectMany(p => p.ParameterType.GetProperties()
            .Where(prop => prop.GetCustomAttribute<JsonIgnoreAttribute>() != null));

        if (ignoredProperties.Any())
        {
            foreach (var property in ignoredProperties)
            {
                operation.Parameters = operation.Parameters
                    .Where(p => !p.Name.Equals(property.Name, StringComparison.InvariantCulture) &&
                                !p.Name.StartsWith(property.Name + ".", StringComparison.InvariantCulture))
                    .ToList();


            }
        }
    }
}