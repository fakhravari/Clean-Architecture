using Application.Services.Serilog;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts
{
    public interface IUnitOfWork
    {
        void SetDatabaseMode(DatabaseMode mode);
        Task<int> SaveChangesAsync();
    }

    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContextFactory _contextFactory;
        private readonly ISerilogService _logger;
        private FakhravariDbContext _context;
        private DatabaseMode _currentMode;

        public UnitOfWork(IDbContextFactory contextFactory, ISerilogService logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        private FakhravariDbContext Context
        {
            get
            {
                if (_context == null)
                    throw new InvalidOperationException("Context is not set. Please call SetDatabaseMode first.");
                return _context;
            }
        }

        public void SetDatabaseMode(DatabaseMode mode)
        {
            if (_context == null || _currentMode != mode)
            {
                _currentMode = mode;
                _context = _contextFactory.CreateDbContext(mode);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                var con = Context.Database.GetConnectionString();

                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }
    }
}
