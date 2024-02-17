using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions;
using MongoDB.Driver;

namespace Magicube.Data.Mongodb {
    public class MongoDbContext : IMongoDbContext {
        public IMongoDatabase Database { get; }
        
        public MongoDbContext(MongoProvider provider) {
            Database = provider.Database;
        }

        public IQueryable<TEntity> Set<TEntity,TKey>() where TEntity : class, IEntity<TKey> {
            var accessor = TypeAccessor.Get<TEntity>();
            var attr = accessor.Context.Attributes.OfType<TableAttribute>().FirstOrDefault();
            string tableName;
            if (attr != null && !attr.Name.IsNullOrEmpty()) {
                tableName = attr.Name;
            } else {
                tableName = accessor.Context.Name;
            }
            return Database.GetCollection<TEntity>(tableName).AsQueryable();
        }

        public IQueryable<TEntity> Set<TEntity>() where TEntity : class, IEntity {
            var accessor = TypeAccessor.Get<TEntity>();
            var attr = accessor.Context.Attributes.OfType<TableAttribute>().FirstOrDefault();
            string tableName;
            if (attr != null && !attr.Name.IsNullOrEmpty()) {
                tableName = attr.Name;
            }
            else {
                tableName = accessor.Context.Name;
            }
            return Database.GetCollection<TEntity>(tableName).AsQueryable();
        }

        public int SaveChanges() => 1;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }
}
