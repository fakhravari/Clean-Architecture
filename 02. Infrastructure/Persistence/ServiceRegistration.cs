using Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repository;

namespace Persistence
{
    public static class ServiceRegistration
    {
        public static void Add_Persistence_Services(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FakhravariDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnectionString_01")));

            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepository<>));

            services.AddScoped<IPersonelRepository, PersonelRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
