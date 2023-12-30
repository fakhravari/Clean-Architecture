using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.ActionFilters;

public class SwggerHeaders : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Language",
            In = ParameterLocation.Header,
            Description= "Persian = 1, English = 2, Arabic = 3",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Enum = new List<IOpenApiAny>
                {
                    new OpenApiString("1"),
                    new OpenApiString("2"),
                    new OpenApiString("3")
                }
            },
            
        });


        var allowAnonymous = false; //context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAPIAttribute>().Any();
        if (allowAnonymous)
            operation.Summary = "Authorization Needed";

    }
}