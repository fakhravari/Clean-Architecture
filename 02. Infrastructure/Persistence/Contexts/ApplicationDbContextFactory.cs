using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts
{
    public interface IDbContextFactory
    {
        FakhravariDbContext CreateDbContext(bool isReadOnly);
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public DbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FakhravariDbContext CreateDbContext(bool isReadOnly)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FakhravariDbContext>();
            var connectionString = isReadOnly ? _configuration.GetConnectionString("ReadDatabase") : _configuration.GetConnectionString("WriteDatabase");

            optionsBuilder.UseSqlServer(connectionString);
            return new FakhravariDbContext(optionsBuilder.Options);
        }
    }

}
