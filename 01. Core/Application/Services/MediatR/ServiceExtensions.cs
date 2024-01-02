using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.MediatR
{
    public static class ServiceExtensions
    {
        public static void AddMediatR_FluentService(this IServiceCollection services, string AssemblyName)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure;
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load(AssemblyName)));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
    }
}
