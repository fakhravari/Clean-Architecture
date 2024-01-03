using System.Reflection;
using Application.Features.Account.Queries.Login;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.MediatR
{
    public static class ServiceExtensions
    {
        public static void AddMediatR_FluentService(this IServiceCollection services, string AssemblyName)
        {
            // ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure;
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load(AssemblyName)));






            services.AddScoped<IRequestHandler<LoginCommand, LoginResponseDto>, LoginQueryHandler>();


        }
    }
}