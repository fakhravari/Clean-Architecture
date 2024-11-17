using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Pluralize.NET;
using Shared.ExtensionMethod;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WebApi.Infrastructure.Swagger
{
    public class SwaggerApplyOperationFilter
    {
        public class ApplySummariesOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
                if (controllerActionDescriptor == null) return;

                var pluralizer = new Pluralizer();

                var actionName = controllerActionDescriptor.ActionName;
                var singularizeName = pluralizer.Singularize(controllerActionDescriptor.ControllerName);
                var pluralizeName = pluralizer.Pluralize(singularizeName);

                var parameterCount = operation.Parameters.Where(p => p.Name != "version" && p.Name != "api-version").Count();

                if (IsGetAllAction())
                {
                    if (!operation.Summary.HasValue())
                        operation.Summary = $"Returns all {pluralizeName}";
                }
                else if (IsActionName("Post", "Create"))
                {
                    if (!operation.Summary.HasValue())
                        operation.Summary = $"Creates a {singularizeName}";
                }
                else if (IsActionName("Read", "Get"))
                {
                    if (!operation.Summary.HasValue())
                        operation.Summary = $"Retrieves a {singularizeName} by unique id";

                    if (!operation.Parameters[0].Description.HasValue())
                        operation.Parameters[0].Description = $"a unique id for the {singularizeName}";
                }
                else if (IsActionName("Put", "Edit", "Update"))
                {
                    if (!operation.Summary.HasValue())
                        operation.Summary = $"Updates a {singularizeName} by unique id";
                }
                else if (IsActionName("Delete", "Remove"))
                {
                    if (!operation.Summary.HasValue())
                        operation.Summary = $"Deletes a {singularizeName} by unique id";

                    if (!operation.Parameters[0].Description.HasValue())
                        operation.Parameters[0].Description = $"A unique id for the {singularizeName}";
                }

                #region Local Functions
                bool IsGetAllAction()
                {
                    foreach (var name in new[] { "Get", "Read", "Select" })
                    {
                        if (actionName.Equals(name, StringComparison.OrdinalIgnoreCase) && parameterCount == 0 ||
                            actionName.Equals($"{name}All", StringComparison.OrdinalIgnoreCase) ||
                            actionName.Equals($"{name}{pluralizeName}", StringComparison.OrdinalIgnoreCase) ||
                            actionName.Equals($"{name}All{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
                            actionName.Equals($"{name}All{pluralizeName}", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    return false;
                }

                bool IsActionName(params string[] names)
                {
                    foreach (var name in names)
                    {
                        if (actionName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                            actionName.Equals($"{name}ById", StringComparison.OrdinalIgnoreCase) ||
                            actionName.Equals($"{name}{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
                            actionName.Equals($"{name}{singularizeName}ById", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                #endregion
            }
        }
        public class ApplyUploadFileOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var fileUploadMime = "multipart/form-data";
                if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
                    return;

                var fileParams = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));
                operation.RequestBody.Content[fileUploadMime].Schema.Properties = fileParams.ToDictionary(k => k.Name, v => new OpenApiSchema() { Type = "string", Format = "binary" });
            }
        }
        public class SwggerHeaders : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Accept-Language",
                    In = ParameterLocation.Header,
                    Description = "Persian = fa-IR, English = en-US, Arabic = ar-IQ",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Enum = new List<IOpenApiAny>
                        {
                            new OpenApiString("fa-IR"),
                            new OpenApiString("en-US"),
                            new OpenApiString("ar-IQ")
                        }
                    }
                });
                //operation.Parameters.Add(new OpenApiParameter
                //{
                //    Name = "X-Token-JWT",
                //    In = ParameterLocation.Header,
                //    Description = "Access Token Key",
                //    Required = true,
                //    Schema = new OpenApiSchema
                //    {
                //        Type = "string",
                //        Default = new OpenApiString("fakhravari.ir")
                //    }
                //});

                var allowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any();
                if (allowAnonymous)
                    operation.Summary = "Authorization Needed";
            }
        }
        public class SwaggerJsonIgnore : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var ignoredProperties = context.MethodInfo.GetParameters().SelectMany(p => p.ParameterType.GetProperties().Where(prop => prop.GetCustomAttribute<JsonIgnoreAttribute>() != null));

                if (ignoredProperties.Any())
                {
                    foreach (var property in ignoredProperties)
                    {
                        operation.Parameters = operation.Parameters.Where(p => !p.Name.Equals(property.Name, StringComparison.InvariantCulture) 
                                                                               && !p.Name.StartsWith(property.Name + ".", StringComparison.InvariantCulture)).ToList();
                    }
                }
            }
        }
        public class AuthorizationOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
                var isAuthorized = actionMetadata.Any(metadataItem => metadataItem is AuthorizeAttribute);
                var allowAnonymous = actionMetadata.Any(metadataItem => metadataItem is AllowAnonymousAttribute);
                if (!isAuthorized || allowAnonymous)
                {
                    return;
                }

                operation.Parameters ??= new List<OpenApiParameter>();
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
            }
        }


        public class RemoveVersionParameters : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "version");
                if (versionParameter != null)
                    operation.Parameters.Remove(versionParameter);
            }
        }
        public class SetVersionInPaths : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                var updatedPaths = new OpenApiPaths();

                foreach (var entry in swaggerDoc.Paths)
                {
                    updatedPaths.Add(entry.Key.Replace("v{version}", swaggerDoc.Info.Version), entry.Value);
                }

                swaggerDoc.Paths = updatedPaths;
            }
        }
    }
}
