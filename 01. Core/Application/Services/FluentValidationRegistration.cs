using Application.Features.Account.Queries.Login;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class FluentValidationRegistration
    {
        public static IServiceCollection AddFluentValidationRegistration(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<LoginQueryValidator>();

  

            //services.AddControllers(options =>
            //{
            //    options.Filters.Add(typeof(ValidationExceptionFilterAttribute));
            //});

            return services;
        }
    }
}