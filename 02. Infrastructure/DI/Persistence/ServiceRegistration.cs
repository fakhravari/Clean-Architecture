using Application.Contracts.Persistence.Contexts;
using Application.Contracts.Persistence.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repository;

namespace DI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddServices_Persistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FakhravariDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ReadDatabase")));

            services.AddSingleton<IDbContextFactory, DbContextFactory>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IPersonelRepository, PersonelRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
