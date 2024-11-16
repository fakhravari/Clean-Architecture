using Application.Contracts.Persistence.Contexts;
using Application.Services.Serilog;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Repository.Generic
{
    public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ISerilogService _logger;
        private TContext? _context;
        private DatabaseMode _currentMode;

        public UnitOfWork(ISerilogService logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public DatabaseMode Mode => _currentMode;
        public TContext Context => _context ??= CreateContext(DatabaseMode.Read);

        public void SetDatabaseMode(DatabaseMode mode)
        {
            var isChange = false;

            if (_context == null)
            {
                isChange = true;
            }
            else if ((mode != _currentMode) && (_configuration.GetConnectionString("ReadDatabase") != _configuration.GetConnectionString("WriteDatabase")))
            {
                isChange = true;
            }

            if (!isChange) return;

            _context?.Dispose();
            _context = CreateContext(mode);
            _currentMode = mode;
        }

        private TContext CreateContext(DatabaseMode mode)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            var connectionString = mode == DatabaseMode.Read
                ? _configuration.GetConnectionString("ReadDatabase")
                : _configuration.GetConnectionString("WriteDatabase");

            optionsBuilder.UseSqlServer(connectionString);
            return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex, additionalInfo: $"DatabaseMode: {_currentMode}");
                throw;
            }
        }
    }
}
