using Application.Services.Serilog;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts
{
    public interface IUnitOfWork
    {
        void SetDatabaseMode(DatabaseMode mode);
        DatabaseMode Mode { get; }
        FakhravariDbContext Context { get; }
        Task<int> SaveChangesAsync();
    }

    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private readonly ISerilogService _logger;
        private FakhravariDbContext _context;
        public DatabaseMode? _currentMode;

        public UnitOfWork(ISerilogService logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            if (_currentMode == null)
            {
                SetDatabaseMode(DatabaseMode.Read);
            }
        }

        public DatabaseMode Mode => _currentMode.Value;
        public FakhravariDbContext Context => _context;

        public void SetDatabaseMode(DatabaseMode mode)
        {
            if (mode == _currentMode)
            {
                return;
            }

            var optionsBuilder = new DbContextOptionsBuilder<FakhravariDbContext>();
            var connectionString = mode == DatabaseMode.Read ? _configuration.GetConnectionString("ReadDatabase") : _configuration.GetConnectionString("WriteDatabase");

            optionsBuilder.UseSqlServer(connectionString);
            _context = new FakhravariDbContext(optionsBuilder.Options);
            _currentMode = mode;
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
