using LiteDB;
using LiteDB.Queryable;
using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Data.LiteDb {
    public interface ILiteDbContext : IDbContext {
		ILiteCollection<T> Collection<T>();
    }

	public class LiteDbContext : ILiteDbContext {
		private readonly ILiteDatabase _database;
		private readonly DatabaseOptions _options;

        public LiteDbContext(IOptionsMonitor<DatabaseOptions> options) {
			_database         = new LiteDatabase(options.CurrentValue.Value);
			_options          = options.CurrentValue;
        }

		public ILiteCollection<TEntity> Collection<TEntity>() {
            var accessor = TypeAccessor.Get<TEntity>();
			var attr     = accessor.Context.Attributes.OfType<TableAttribute>().FirstOrDefault();
			string tableName;
			if (attr != null && !attr.Name.IsNullOrEmpty()) {
				tableName = attr.Name;
			} else {
				tableName = accessor.Context.Name;
			}

			var collection = _database.GetCollection<TEntity>(tableName);
			
			InitIndexes(collection);

			foreach(var item in _options.EntityConfs) {
				if (item.Value != null) {
					foreach(var it in item.Value) {
						var conf = New<IMappingConfiguration>.Creator(it);
						conf.ApplyConfiguration(BsonMapper.Global);
					}
				}
			}

			return collection;
		}

		IQueryable<TEntity> IDbContext.Set<TEntity, TKey>() {
			return Collection<TEntity>().AsQueryable();
		}

        IQueryable<TEntity> IDbContext.Set<TEntity>() {
            return Collection<TEntity>().AsQueryable();
        }

        public int SaveChanges() => 1;

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

		private void InitIndexes<TEntity>(ILiteCollection<TEntity> collection) {
			var item = typeof(TEntity);
			var typeAccessor = TypeAccessor.Get(item, null);
			foreach (var propertyInfo in typeAccessor.Context.Properties) {
				var indexAttr = propertyInfo.GetAttribute<IndexFieldAttribute>();
                if (indexAttr != null) {
					collection.EnsureIndex(indexAttr.Name ?? propertyInfo.Member.Name, indexAttr.IsUnique);
                }
			}
		}
	}
}