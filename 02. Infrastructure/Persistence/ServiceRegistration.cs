using Application.Contracts.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repository;

namespace Persistence
{
    public static class ServiceRegistration
    {
        public static void AddServices_Persistence(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationDbContextFactory, ApplicationDbContextFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IPersonelRepository, PersonelRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
