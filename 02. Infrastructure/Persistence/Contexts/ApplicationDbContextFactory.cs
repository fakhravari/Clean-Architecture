using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts
{
    public interface IApplicationDbContextFactory
    {
        FakhravariDbContext CreateDbContext(bool isReadOnly);
    }

    public class ApplicationDbContextFactory : IApplicationDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FakhravariDbContext CreateDbContext(bool isReadOnly)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FakhravariDbContext>();
            var connectionString = isReadOnly
                ? _configuration.GetConnectionString("ReadDatabase")
                : _configuration.GetConnectionString("WriteDatabase");

            optionsBuilder.UseSqlServer(connectionString);

            return new FakhravariDbContext(optionsBuilder.Options);
        }
    }

}
