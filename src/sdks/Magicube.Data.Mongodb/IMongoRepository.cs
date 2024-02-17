using Magicube.Data.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Data.Mongodb {
    public interface IMongoRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Entity<TKey> {
        IMongoCollection<TEntity> Collection { get; }

        IMongoCollection<TEntity> GetCollection(string name);

        Task UpdateAsync(BsonValue k, UpdateDefinition<TEntity> update);

        Task<IEnumerable<TEntity>> GetsAsync(int limit, FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort = null);
    }
}
