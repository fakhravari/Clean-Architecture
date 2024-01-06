using Application.ExceptionsHandler;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Services.MediatR
{
    public static class ServiceExtensions
    {
        public static void Add_MediatR_Fluent_ApiResult_Service(this IServiceCollection services)
        {
            #region راه انداز MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            #endregion

            #region راه انداز ولیدیشن
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            #region داینامیک اضافه کردن نوع ولیدیشن
            var assembly = Assembly.GetExecutingAssembly();
            var validatorType = typeof(IValidator<>);
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && !type.IsGenericType)
                {
                    var interfaces = type.GetInterfaces();
                    var validatorInterface = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType);

                    if (validatorInterface != null)
                    {
                        services.AddTransient(validatorInterface, type);
                    }
                }
            }
            #endregion

            #endregion

            #region یکسان سازی خروجی
            // اعمال Exception Validation در خروجی سمت Api
            services.AddScoped<ApiResultValidationException>();
            services.AddControllers(options => { options.Filters.AddService<ApiResultValidationException>(); });

            // اعمال Exception در خروجی سمت Api
            services.AddScoped<ApiResultException>();
            services.AddControllers(options => { options.Filters.AddService<ApiResultException>(); });
            #endregion
        }
    }
}