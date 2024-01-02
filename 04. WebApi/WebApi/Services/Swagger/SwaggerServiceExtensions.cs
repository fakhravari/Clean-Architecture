using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace WebApi.Services.Swagger
{
    public static class SwaggerServiceExtensions
    {
        public static void AddSwaggerService(this IServiceCollection services)
        {
            #region AddSwaggerExamples
            var mainAssembly = Assembly.GetEntryAssembly();
            var mainType = mainAssembly.GetExportedTypes()[0];

            const string methodName = nameof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions.AddSwaggerExamplesFromAssemblyOf);
            MethodInfo method = typeof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions).GetRuntimeMethods().FirstOrDefault(x => x.Name == methodName && x.IsGenericMethod);
            MethodInfo generic = method.MakeGenericMethod(mainType);
            generic.Invoke(null, new[] { services });
            #endregion

            services.AddApiVersioning(apiVersioningOptions =>
            {
                apiVersioningOptions.ReportApiVersions = true;
                apiVersioningOptions.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "MyApi.xml"), true);
                options.EnableAnnotations();

                options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API V1", Description = "API V1 Description" });
                options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "API V2", Description = "API V2 Description" });

                #region Filters
                options.ExampleFilters();

                options.OperationFilter<SwaggerApplyOperationFilter.ApplySummariesOperationFilter>();
                options.OperationFilter<SwaggerApplyOperationFilter.ApplyUploadFileOperationFilter>();
                options.OperationFilter<SwaggerApplyOperationFilter.SwggerHeaders>();
                options.OperationFilter<SwaggerApplyOperationFilter.SwaggerJsonIgnore>();


                options.OperationFilter<SwaggerApplyOperationFilter.UnauthorizedResponsesOperationFilter>();
                #region Versioning
                options.OperationFilter<SwaggerApplyOperationFilter.RemoveVersionParameters>();
                options.DocumentFilter<SwaggerApplyOperationFilter.SetVersionInPaths>();
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
                    var versions = methodInfo.DeclaringType.GetCustomAttributes<ApiVersionAttribute>(true).SelectMany(attr => attr.Versions);
                    return versions.Any(v => $"v{v}" == docName);
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "IdToken",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                    BearerFormat = "JWT"
                });
                options.OperationFilter<SwaggerApplyOperationFilter.AuthorizationOperationFilter>();
                #endregion

                #endregion
            });
            services.AddApiVersioning(options =>
            {
                //url segment => {version}
                options.AssumeDefaultVersionWhenUnspecified = true; //default => false;
                options.DefaultApiVersion = new ApiVersion(1, 0); //v1.0 == v1
                options.ReportApiVersions = true;
            });
        }
        public static IApplicationBuilder UseSwaggerAndUI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"../swagger/v1/swagger.json", "V1 Version");
                options.SwaggerEndpoint($"../swagger/v2/swagger.json", "V2 Version");
                #region Customizing
                //// Display
                //options.DefaultModelExpandDepth(2);
                //options.DefaultModelRendering(ModelRendering.Model);
                //options.DefaultModelsExpandDepth(-1);
                //options.DisplayOperationId();
                //options.DisplayRequestDuration();
                options.DocExpansion(DocExpansion.None);
                //options.EnableDeepLinking();
                //options.EnableFilter();
                //options.MaxDisplayedTags(5);
                //options.ShowExtensions();

                //// Network
                //options.EnableValidator();
                //options.SupportedSubmitMethods(SubmitMethod.Get);

                //// Other
                //options.DocumentTitle = "CustomUIConfig";
                //options.InjectStylesheet("/ext/custom-stylesheet.css");
                //options.InjectJavascript("/ext/custom-javascript.js");
                //options.RoutePrefix = "api-docs";
                #endregion
            });

            app.UseReDoc(options =>
            {
                options.SpecUrl("../swagger/v1/swagger.json");
                #region Customizing
                options.DocumentTitle = "My API Docs";

                options.EnableUntrustedSpec();
                options.ScrollYOffset(10);
                options.HideHostname();
                options.HideDownloadButton();
                options.ExpandResponses("200,201");
                options.RequiredPropsFirst();
                options.NoAutoAuth();
                options.PathInMiddlePanel();
                options.HideLoading();
                options.NativeScrollbars();
                options.DisableSearch();
                options.OnlyRequiredInSamples();
                options.SortPropsAlphabetically();
                #endregion
            });

            return app;
        }
    }
}
