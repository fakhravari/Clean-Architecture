using Application.Contracts.Persistence.Contexts;
using Application.Contracts.Persistence.IRepository;
using Application.Services.Serilog;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;

namespace Persistence.Contexts
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ISerilogService _logger;
        private IDbContextTransaction? _transaction;
        private IUnitOfWork<FakhravariDbContext> _unitOfWork;

        public GenericRepository(IUnitOfWork<FakhravariDbContext> iUnitOfWork, ISerilogService logger)
        {
            _logger = logger;
            _unitOfWork = iUnitOfWork;
        }

        #region Query

        public async Task<TEntity?> QuerySingleAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? query, bool asNoTracking = true)
        {
            try
            {
                var con = _unitOfWork.Context.Database.GetConnectionString();

                var queryable = query(_unitOfWork.Context.Set<TEntity>());
                return await (asNoTracking ? queryable.AsNoTracking() : queryable.AsTracking()).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task<List<T>> QueryListAsync<T>(Func<IQueryable<T>, IQueryable<T>>? query = null, bool asNoTracking = true) where T : class
        {
            try
            {
                var dbSet = query != null ? query(_unitOfWork.Context.Set<T>()) : _unitOfWork.Context.Set<T>().AsQueryable();
                return await (asNoTracking ? dbSet.AsNoTracking() : dbSet.AsTracking()).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task<T?> QuerySingleRawAsync<T>(string tSql, params object[] parameters) where T : class
        {
            try
            {
                return await _unitOfWork.Context.Set<T>().FromSqlRaw(tSql, parameters).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task<List<T>> QueryListRawAsync<T>(string tSql, params object[] parameters) where T : class
        {
            try
            {
                return await _unitOfWork.Context.Set<T>().FromSqlRaw(tSql, parameters).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task<List<IList>> MultipleResultsAsync(string query, List<Type> resultTypes, IEnumerable<SqlParameter>? parameters = null)
        {
            try
            {
                var results = new List<IList>();
                var connection = _unitOfWork.Context.Database.GetDbConnection();
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = query;

                if (parameters != null) command.Parameters.AddRange(parameters.ToArray());

                await using var reader = await command.ExecuteReaderAsync();
                foreach (var resultType in resultTypes)
                {
                    var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(resultType))!;

                    while (await reader.ReadAsync())
                    {
                        var instance = Activator.CreateInstance(resultType);
                        var properties = resultType.GetProperties();

                        foreach (var property in properties)
                        {
                            if (reader.GetOrdinal(property.Name) < 0) continue;

                            if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                            {
                                property.SetValue(instance, reader[property.Name]);
                            }
                        }
                        result.Add(instance);
                    }
                    results.Add(result);
                    if (!await reader.NextResultAsync()) break;
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        #endregion

        #region Add_Edit_Remove

        public async Task AddAsync(TEntity entity)
        {
            try
            {
                await _unitOfWork.Context.AddAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task AddRangeAsync(IList<TEntity> entities)
        {
            try
            {
                var con = _unitOfWork.Context.Database.GetConnectionString();

                await _unitOfWork.Context.AddRangeAsync(entities);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }
        }

        public Task UpdateAsync(TEntity entity)
        {
            try
            {
                _unitOfWork.Context.Update(entity);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }

            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IList<TEntity> entities)
        {
            try
            {
                _unitOfWork.Context.UpdateRange(entities);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(TEntity entity)
        {
            try
            {
                _unitOfWork.Context.Remove(entity);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }

            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IList<TEntity> entities)
        {
            try
            {
                _unitOfWork.Context.RemoveRange(entities);
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
                throw;
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Bulk

        public async Task BulkInsertAsync(IList<TEntity> entities, BulkConfig? config)
        {
            using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Context.BulkInsertAsync(entities, config);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task BulkUpdateAsync(IList<TEntity> entities, BulkConfig? config)
        {
            using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Context.BulkUpdateAsync(entities, config);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task BulkDeleteAsync(IList<TEntity> entities, BulkConfig? config)
        {
            using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Context.BulkDeleteAsync(entities, config);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogSystem(ex);
                throw;
            }
        }

        public async Task BulkInsertOrUpdateAsync(IList<TEntity> entities, BulkConfig? config)
        {
            await using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Context.BulkInsertOrUpdateAsync(entities, config);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogSystem(ex);
                throw;
            }
        }

        #endregion

        #region Transaction

        public async Task BeginTransactionAsync()
        {
            try
            {
                _transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();
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
                await _transaction!.CommitAsync();
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
                if (ex != null) _logger.LogSystem(ex);

                await _transaction!.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            catch (Exception e)
            {
                _logger.LogSystem(e);
                throw;
            }
        }

        #endregion

        //#region Dispose

        //public async ValueTask DisposeAsync()
        //{
        //    try
        //    {
        //        if (_unitOfWork.Context != null)
        //        {
        //            await _unitOfWork.Context.DisposeAsync();
        //        }

        //        if (_transaction != null)
        //        {
        //            await _transaction.DisposeAsync();
        //            _transaction = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogSystem(ex);
        //        throw;
        //    }
        //}

        //#endregion
    }
}
