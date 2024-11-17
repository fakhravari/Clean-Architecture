using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace WebApi.Infrastructure.Swagger
{
    public static class SwaggerServiceExtensions
    {
        public static void AddService_Swagger(this IServiceCollection services)
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
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "X-Token-JWT",
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new List<string>()
                        }
                    });

                    options.OperationFilter<SwaggerApplyOperationFilter.ApplySummariesOperationFilter>();
                    options.OperationFilter<SwaggerApplyOperationFilter.ApplyUploadFileOperationFilter>();
                    options.OperationFilter<SwaggerApplyOperationFilter.SwggerHeaders>();
                    options.OperationFilter<SwaggerApplyOperationFilter.SwaggerJsonIgnore>();


                    // Authorization Ui
                    options.OperationFilter<SwaggerApplyOperationFilter.AuthorizationOperationFilter>();
                    #endregion
                    #region Versioning
                    options.OperationFilter<SwaggerApplyOperationFilter.RemoveVersionParameters>();
                    options.DocumentFilter<SwaggerApplyOperationFilter.SetVersionInPaths>();
                    options.DocInclusionPredicate((docName, apiDesc) =>
                    {
                        if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
                        var versions = methodInfo.DeclaringType.GetCustomAttributes<ApiVersionAttribute>(true).SelectMany(attr => attr.Versions);
                        return versions.Any(v => $"v{v}" == docName);
                    });
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
        public static void UseSwaggerAndUI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"../swagger/v1/swagger.json", "V1 Version");
                options.SwaggerEndpoint($"../swagger/v2/swagger.json", "V2 Version");
                #region Customizing
                //// Display
                //options.DefaultModelExpandDepth(2);
                options.DefaultModelRendering(ModelRendering.Model);
                options.DefaultModelsExpandDepth(-1);
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
                options.DocumentTitle = "My API Documentation";   // عنوان صفحه
                options.SpecUrl = "/swagger/v1/swagger.json";     // آدرس فایل JSON
                options.RoutePrefix = "redoc";                    // مسیر مستندات
                //options.ExpandResponses("200");                  // نمایش پیش‌فرض پاسخ‌های موفق
                //options.HideDownloadButton();                    // مخفی کردن دکمه دانلود
                //options.HideHostname();                          // مخفی کردن هاست
                //options.NoAutoAuth();                            // جلوگیری از ورود خودکار
                //options.ScrollYOffset(100);                      // تنظیم میزان اسکرول
                //options.RequiredPropsFirst();                    // نمایش ویژگی‌های ضروری در ابتدا
                //options.PathInMiddlePanel();                     // نمایش مسیرها در ستون میانی
                //options.EnableUntrustedSpec();                   // پذیرش منابع غیرقابل اعتماد
                //options.NativeScrollbars();                      // استفاده از اسکرول بومی
            });
        }
    }
}
