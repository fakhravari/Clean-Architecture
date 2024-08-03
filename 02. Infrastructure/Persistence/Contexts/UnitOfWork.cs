using Application.Contracts.Persistence.Contexts;
using Application.Services.Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Persistence.Contexts
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContextFactory _contextFactory;
        private DbContext _context;
        private IDbContextTransaction _transaction;
        private readonly ISerilogService _logger;

        public UnitOfWork(IDbContextFactory contextFactory, ISerilogService logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public void CreateContext(bool isReadOnly)
        {
            DisposeContext();
            _context = _contextFactory.CreateDbContext(isReadOnly);
        }

        private void DisposeContext()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public DbContext Context => _context;

        public async Task<T> QuerySingleAsync<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : class
        {
            try
            {
                return await query(_context.Set<T>()).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task<List<T>> QueryListAsync<T>(Func<IQueryable<T>, IQueryable<T>> query = null) where T : class
        {
            try
            {
                var dbSet = _context.Set<T>().AsNoTracking();
                return query != null ? await query(dbSet).ToListAsync() : await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task<T> QuerySingleRawAsync<T>(string sql, params object[] parameters) where T : class
        {
            try
            {
                return await _context.Set<T>().FromSqlRaw(sql, parameters).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task<List<T>> QueryListRawAsync<T>(string sql, params object[] parameters) where T : class
        {
            try
            {
                return await _context.Set<T>().FromSqlRaw(sql, parameters).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            try
            {
                _context.Set<T>().Add(entity);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            try
            {
                _context.Set<T>().Update(entity);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            try
            {
                _context.Set<T>().Remove(entity);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task BeginTransactionAsync()
        {
            try
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task RollbackTransactionAsync(Exception? ex)
        {
            try
            {
                if (ex != null)
                {
                    _logger.LogSystem(ex);
                }

                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            catch (Exception e)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_context != null)
                {
                    await _context.DisposeAsync();
                    _context = null;
                }
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }
    }
}
