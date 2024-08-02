using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Persistence
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        void CreateContext(bool isReadOnly);
        DbContext Context { get; }


        Task<T> QuerySingleAsync<T>(Func<IQueryable<T>, IQueryable<T>> query = null) where T : class;
        Task<List<T>> QueryListAsync<T>(Func<IQueryable<T>, IQueryable<T>> query = null) where T : class;
        Task<T> QuerySingleRawAsync<T>(string sql, params object[] parameters) where T : class;
        Task<List<T>> QueryListRawAsync<T>(string sql, params object[] parameters) where T : class;



        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}
