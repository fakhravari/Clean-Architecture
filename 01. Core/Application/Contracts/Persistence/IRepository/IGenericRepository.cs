using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace Application.Contracts.Persistence.IRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> QuerySingleAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? query = null, bool asNoTracking = true);
        Task<List<T>> QueryListAsync<T>(Func<IQueryable<T>, IQueryable<T>>? query = null, bool asNoTracking = true) where T : class;
        Task<List<IList>> MultipleResultsAsync(string query, List<Type> resultTypes, IEnumerable<SqlParameter>? parameters = null);


        Task<T?> QuerySingleRawAsync<T>(string tSql, params object[] parameters) where T : class;
        Task<List<T>> QueryListRawAsync<T>(string tSql, params object[] parameters) where T : class;



        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IList<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task UpdateRangeAsync(IList<TEntity> entities);
        Task RemoveAsync(TEntity entity);
        Task RemoveRangeAsync(IList<TEntity> entities);



        Task BulkInsertAsync(IList<TEntity> entities, BulkConfig? config);
        Task BulkUpdateAsync(IList<TEntity> entities, BulkConfig? config);
        Task BulkDeleteAsync(IList<TEntity> entities, BulkConfig? config);
        Task BulkInsertOrUpdateAsync(IList<TEntity> entities, BulkConfig? config);


        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync(Exception? ex);
    }

}
