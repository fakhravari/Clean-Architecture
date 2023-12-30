using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Config.Swagger
{
    public class RemoveVersionParameters : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "version");
            if (versionParameter != null)
                operation.Parameters.Remove(versionParameter);
        }
    }
}
