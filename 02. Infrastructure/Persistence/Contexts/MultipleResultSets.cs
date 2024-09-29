using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Persistence.Contexts;

public static class MultipleResultSets
{
    public static MultipleResultSetWrapper MultipleResults(this DbContext db, string query, IEnumerable<SqlParameter> parameters = null)
    {
        return new MultipleResultSetWrapper(db, query, parameters);
    }

    public class MultipleResultSetWrapper
    {
        private readonly DbContext _db;
        private readonly string _CommandText;
        private readonly IEnumerable<SqlParameter> _parameters;
        public List<Func<DbDataReader, IEnumerable>> _resultSets;

        public MultipleResultSetWrapper(DbContext db, string query, IEnumerable<SqlParameter> parameters = null)
        {
            _db = db;
            _CommandText = query;
            _parameters = parameters;
            _resultSets = new List<Func<DbDataReader, IEnumerable>>();
        }

        public MultipleResultSetWrapper AddResult<TResult>()
        {
            _resultSets.Add(OneResult<TResult>);
            return this;
        }
        public async Task<List<IEnumerable>> ExecuteAsync()
        {
            var results = new List<IEnumerable>();

            using (var connection = _db.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = _CommandText;
                    if (_parameters.Any())
                    {
                        command.Parameters.AddRange(_parameters.ToArray());
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        foreach (var resultSet in _resultSets)
                        {
                            var result = resultSet(reader);
                            results.Add(result);

                            if (!await reader.NextResultAsync()) break;
                        }
                    }
                }
            }

            return results;
        }
        private IEnumerable OneResult<TResult>(DbDataReader reader)
        {
            var result = new List<TResult>();
            var properties = typeof(TResult).GetProperties();

            while (reader.Read())
            {
                var instance = Activator.CreateInstance<TResult>();
                bool hasData = false;

                foreach (var property in properties)
                {
                    if (HasColumn(reader, property.Name))
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            var value = reader[property.Name];
                            property.SetValue(instance, value);
                            hasData = true;
                        }
                    }
                }

                if (hasData)
                {
                    result.Add(instance);
                }
            }

            reader.NextResult();
            return result;
        }


        private static bool HasColumn(IDataRecord reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }
    }
}
