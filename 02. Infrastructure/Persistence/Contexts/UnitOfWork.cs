using Application.Services.Serilog;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts
{
    public interface IUnitOfWork
    {
        FakhravariDbContext SetDatabaseMode(DatabaseMode mode);
        Task<int> SaveChangesAsync();
    }

    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private readonly ISerilogService _logger;
        private FakhravariDbContext _context;
        private DatabaseMode? _currentMode = null;

        public UnitOfWork(ISerilogService logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        public FakhravariDbContext SetDatabaseMode(DatabaseMode mode)
        {
            if (mode == _currentMode)
            {
                return this._context;
            }

            var optionsBuilder = new DbContextOptionsBuilder<FakhravariDbContext>();
            var connectionString = mode == DatabaseMode.Read ? _configuration.GetConnectionString("ReadDatabase") : _configuration.GetConnectionString("WriteDatabase");

            optionsBuilder.UseSqlServer(connectionString);
            _context = new FakhravariDbContext(optionsBuilder.Options);
            _currentMode = mode;

            return _context;
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                var con = _context.Database.GetConnectionString();

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }
    }
}
