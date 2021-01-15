using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace CatalogApi.DAL
{
    public interface IQueries
    {
        Task<IEnumerable<T>> GetAsync<T>() where T : Catalog;

        Task<T> GetAsync<T>(int id) where T : Catalog;
    }

    public class Queries : IQueries
    {
        private readonly string _connectionString;
        private readonly EntitiesInfoService _entitiesInfoService;

        public Queries(string connectionString, EntitiesInfoService entitiesInfoService)
        {
            _connectionString = connectionString;
            _entitiesInfoService = entitiesInfoService;
        }

        public DbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<T>> GetAsync<T>() where T : Catalog
        {
            var info = _entitiesInfoService[typeof(T)];

            IEnumerable<T> entities;

            using (var connection = Connection)
            {
                entities = await connection.QueryAsync<T>($"SELECT * FROM public.\"{info.TableName}\"");
            }

            return entities;
        }

        public async Task<T> GetAsync<T>(int id) where T : Catalog
        {
            var info = _entitiesInfoService[typeof(T)];

            T entity;

            using (var connection = Connection)
            {
                entity = await connection.ExecuteScalarAsync<T>(
                    $"SELECT * FROM public.\"{info.TableName}\" WHERE Id = {id}");
            }

            return entity;
        }
    }
}
