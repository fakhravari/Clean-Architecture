using Application.Contracts.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence.Contexts
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContextFactory _contextFactory;
        private DbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void CreateContext(bool isReadOnly)
        {
            _context = _contextFactory.CreateDbContext(isReadOnly);
        }

        public DbContext Context => _context;




        public async Task<T> QuerySingleAsync<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : class
        {
            return await query(_context.Set<T>()).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<List<T>> QueryListAsync<T>(Func<IQueryable<T>, IQueryable<T>> query = null) where T : class
        {
            var dbSet = _context.Set<T>().AsNoTracking();
            var result = query != null ? await query(dbSet).ToListAsync() : await dbSet.ToListAsync();
            return result;
        }

        public async Task<T> QuerySingleRawAsync<T>(string sql, params object[] parameters) where T : class
        {
            return await _context.Set<T>().FromSqlRaw(sql, parameters).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<List<T>> QueryListRawAsync<T>(string sql, params object[] parameters) where T : class
        {
            return await _context.Set<T>().FromSqlRaw(sql, parameters).AsNoTracking().ToListAsync();
        }





        public async Task AddAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            await SaveChangesAsync();
        }






        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }
            await _transaction.CommitAsync();
            _transaction.Dispose();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }
            await _transaction.RollbackAsync();
            _transaction.Dispose();
            _transaction = null;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
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
    }
}
