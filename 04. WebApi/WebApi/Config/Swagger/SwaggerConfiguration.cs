using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;
using static WebApi.Config.Swagger.SwaggerConfiguration;

namespace WebApi.Config.Swagger
{

    public static class SwaggerConfiguration
    {
        public static string GetCamelCase(this string str) => char.ToLowerInvariant(str[0]) + str[1..];
        public class JsonIgnoreSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                var propertiesToRemove = context.Type.GetProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() != null).Select(p => p.Name.GetCamelCase()).ToList();

                foreach (var propertyToRemove in propertiesToRemove)
                {
                    schema.Properties.Remove(propertyToRemove);
                }
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
                        operation.Parameters = operation.Parameters
                            .Where(p => !p.Name.Equals(property.Name, StringComparison.InvariantCulture) && !p.Name.StartsWith(property.Name + ".", StringComparison.InvariantCulture)).ToList();

                    }
                }
            }
        }



        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class AllowAnonymousAPIAttribute : Attribute
        {
            public AllowAnonymousAPIAttribute()
            {

            }
        }


        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class AuthorizeAPIAttribute : Attribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAPIAttribute>().Any();
                if (allowAnonymous)
                    return;

                var token = context.HttpContext.Request.Headers["X-token"].ToString();
                if (token is null)
                {
                    context.Result = new JsonResult(new
                    {
                        message = "Z_Shoma_Ehraz_Hoviat_Nashodid"
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
            }
        }



        public class SwggerHeaders : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Language",
                    In = ParameterLocation.Header,
                    Description = "Persian = 1, English = 2, Arabic = 3",
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


                var allowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAPIAttribute>().Any();
                if (allowAnonymous)
                    operation.Summary = "Authorization Needed";

            }
        }

        public static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "MyApi.xml"));

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Project", Version = "v2" });
                c.DocInclusionPredicate((doc, apiDescription) =>
                {
                    if (!apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var version = methodInfo.DeclaringType.GetCustomAttributes<ApiVersionAttribute>(true).SelectMany(attr => attr.Versions);
                    return version.Any(v => $"v{v.ToString()}" == doc);
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "X-token",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                });
                c.SchemaFilter<JsonIgnoreSchemaFilter>();
                c.OperationFilter<SwaggerJsonIgnore>();
                 c.OperationFilter<SwggerHeaders>();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme,Id = "Bearer"}
                    },
                    Array.Empty<string>()
                }
            });
            });

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
        }
    }
}