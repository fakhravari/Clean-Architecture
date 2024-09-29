using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts
{
    public interface IDbContextFactory
    {
        FakhravariDbContext CreateDbContext(DatabaseMode mode);
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public DbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FakhravariDbContext CreateDbContext(DatabaseMode mode)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FakhravariDbContext>();
            var connectionString = mode == DatabaseMode.Read ? _configuration.GetConnectionString("ReadDatabase") : _configuration.GetConnectionString("WriteDatabase");

            optionsBuilder.UseSqlServer(connectionString);
            var Context = new FakhravariDbContext(optionsBuilder.Options);
            return Context;
        }
    }

}
