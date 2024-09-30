using Application.Contracts.Persistence.Contexts;
using Application.Services.Serilog;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts
{
    public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ISerilogService _logger;
        private TContext _context;
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
        public TContext Context => _context;

        public void SetDatabaseMode(DatabaseMode mode)
        {
            if (mode == _currentMode)
            {
                return;
            }

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            var connectionString = mode == DatabaseMode.Read
                ? _configuration.GetConnectionString("ReadDatabase")
                : _configuration.GetConnectionString("WriteDatabase");

            optionsBuilder.UseSqlServer(connectionString);
            _context = (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
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
