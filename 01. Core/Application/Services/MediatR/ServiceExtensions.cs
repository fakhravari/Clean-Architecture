using Application.Features.Account.Commands.Login;
using Domain.Exception;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Services.MediatR
{
    public static class ServiceExtensions
    {
        public static void AddMediatR_FluentService(this IServiceCollection services)
        {
            #region راه انداز MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            #endregion

            #region راه انداز ولیدیشن
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();


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