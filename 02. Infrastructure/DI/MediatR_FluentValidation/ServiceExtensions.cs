using Application.Services.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DI.MediatR_FluentValidation;

public static class ServiceExtensions
{
    public static void AddService_MediatR_FluentValidation(this IServiceCollection services)
    {
        var assemblyHandler = typeof(RegisterAssembly).Assembly;

        #region راه انداز MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblyHandler));
        #endregion

        #region راه انداز ولیدیشن
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        #region داینامیک اضافه کردن نوع ولیدیشن
        
        var validatorType = typeof(IValidator<>);
        foreach (var type in assemblyHandler.GetTypes())
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
    }
}
