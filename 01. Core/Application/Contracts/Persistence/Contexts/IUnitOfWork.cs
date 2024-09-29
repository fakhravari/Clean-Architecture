using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Application.Contracts.Persistence.Contexts
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        void CreateContext(bool isReadOnly);
        DbContext? Context { get; }


        Task<T?> QuerySingleAsync<T>(Func<IQueryable<T>, IQueryable<T>>? query = null, bool asNoTracking = true) where T : class;
        Task<List<T>> QueryListAsync<T>(Func<IQueryable<T>, IQueryable<T>>? query = null, bool asNoTracking = true) where T : class;

        Task<T?> QuerySingleRawAsync<T>(string tSql, params object[] parameters) where T : class;
        Task<List<T>> QueryListRawAsync<T>(string tSql, params object[] parameters) where T : class;



        Task AddAsync<T>(T entity) where T : class;
        Task AddRangeAsync<T>(IList<T> entities) where T : class;
        Task UpdateAsync<T>(T entity) where T : class;
        Task UpdateRangeAsync<T>(IList<T> entities) where T : class;
        Task RemoveAsync<T>(T entity) where T : class;
        Task RemoveRangeAsync<T>(IList<T> entities) where T : class;



        Task BulkInsertAsync<T>(IList<T> entities, BulkConfig? config) where T : class;
        Task BulkUpdateAsync<T>(IList<T> entities, BulkConfig? config) where T : class;
        Task BulkDeleteAsync<T>(IList<T> entities, BulkConfig? config) where T : class;
        Task BulkInsertOrUpdateAsync<T>(IList<T> entities, BulkConfig? config) where T : class;


        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync(Exception? ex);
        Task<int> SaveChangesAsync();







        Task<List<IList>> MultipleResultsAsync(string query, List<Type> resultTypes, IEnumerable<SqlParameter>? parameters = null);
    }

}
