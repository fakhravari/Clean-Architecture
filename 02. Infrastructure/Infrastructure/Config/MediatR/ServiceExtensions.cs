using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure.Config.MediatR
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
