using Application.Contracts.Persistence.Contexts;
using Application.Contracts.Persistence.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repository;

namespace DI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddServices_Persistence(this IServiceCollection services)
        {
            services.AddSingleton<IDbContextFactory, DbContextFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IPersonelRepository, PersonelRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
