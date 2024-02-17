using Magicube.Core;
using Magicube.Data.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Data.Mongodb {
    public class Repository<TEntity, TKey> : IMongoRepository<TEntity, TKey> where TEntity : Entity<TKey> {
        private static readonly ConcurrentDictionary<Type, EntityInfoCache> FieldsCache = new ConcurrentDictionary<Type, EntityInfoCache>();

        private readonly IMongoDbContext _context;
        private readonly MongoAssistor<TEntity,TKey> _mongoAssistor;
        public Repository(IMongoDbContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context       = context;
            _mongoAssistor = new MongoAssistor<TEntity, TKey>(_context.Database);
            
            Collection     = _context.Database.GetCollection<TEntity>(FieldsCache.GetOrAdd(typeof(TEntity), ParseEntityInfo).TableName);
        }
        public IMongoCollection<TEntity>   Collection { get; }

        public virtual IQueryable<TEntity> All      => Collection.AsQueryable();

        #region Sync
        public virtual void Delete(TEntity entity) {
            var filter = Builders<TEntity>.Filter.Eq("_id", GetKey(entity));
            Collection.DeleteOne(filter);
        }

        public virtual void Delete(TKey id) {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            Collection.DeleteOne(filter);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate) {
            var entities = Query(predicate);
            var listWrites = new List<WriteModel<TEntity>>();
            foreach (var entity in entities) {
                listWrites.Add(new DeleteOneModel<TEntity>(Builders<TEntity>.Filter.Eq("_id", GetKey(entity))));
            }
            Collection.BulkWrite(listWrites);
        }

        public TEntity Get(TKey id) {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            return Collection.Find(filter).FirstOrDefault();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate) {
            return All.FirstOrDefault(predicate);
        }

        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> predicate) {
            return All.Where(predicate).ToArray();
        }

        public TEntity Insert(TEntity entity) {
            if (entity.Id.Equals(default(TKey))) entity.Id = _mongoAssistor.IncreaseId();
            Collection.InsertOne(entity);
            return entity;
        }

        public void Insert(IEnumerable<TEntity> entities) {
            var list = new List<TEntity>();
            foreach(var item in entities) {
                if (item.Id.Equals(default(TKey))) item.Id = _mongoAssistor.IncreaseId();
                list.Add(item);
            }

            Collection.InsertMany(list);
        }

        public TEntity Update(TEntity entity) {
            var filter = Builders<TEntity>.Filter.Eq("_id", GetKey(entity));
            var update = BuildUpdate(entity);
            Collection.FindOneAndUpdate(filter, update);
            return entity;
        }

        public void Update(IEnumerable<TEntity> entities) {
            var listWrites = new List<WriteModel<TEntity>>();

            foreach (var entity in entities) {
                var update = BuildUpdate(entity);
                listWrites.Add(new UpdateOneModel<TEntity>(Builders<TEntity>.Filter.Eq("_id", GetKey(entity)), update));
            }

            Collection.BulkWrite(listWrites);
        }

        #endregion

        #region Async
        public virtual ValueTask<TEntity> GetAsync(TKey id) {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            var result = Collection.Find(filter);
            return new ValueTask<TEntity>(result.FirstOrDefault());
        }

        public virtual Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate) {
            return Collection.AsQueryable().Where(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TEntity>>  QueryAsync(Expression<Func<TEntity, bool>> predicate) {
            return await Collection.AsQueryable().Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>>  GetsAsync(int limit, FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort = null) {
            var find = Collection.Find(filter);
            if (sort != null) {
                find = find.Sort(sort);
            }
            return await find.Limit(limit).ToListAsync();
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) {
            if (entity.Id.Equals(default(TKey))) entity.Id = _mongoAssistor.IncreaseId();
            await Collection.InsertOneAsync(entity, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);
            return entity;
        }

        public virtual async Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
            var list = new List<TEntity>();
            foreach (var item in entities) {
                if (item.Id.Equals(default(TKey))) item.Id = _mongoAssistor.IncreaseId();
                list.Add(item);
            }
            await Collection.InsertManyAsync(list, new InsertManyOptions { BypassDocumentValidation = false }, cancellationToken);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) {
            var filter = Builders<TEntity>.Filter.Eq("_id", GetKey(entity));
            var update = BuildUpdate(entity);
            await Collection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<TEntity, TEntity> { BypassDocumentValidation = false, IsUpsert = true }, cancellationToken);
            return entity;
        }

        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
            var listWrites = new List<WriteModel<TEntity>>();

            foreach (var entity in entities) {
                var update = BuildUpdate(entity);
                listWrites.Add(new UpdateOneModel<TEntity>(Builders<TEntity>.Filter.Eq("_id", GetKey(entity)), update));
            }

            await Collection.BulkWriteAsync(listWrites, new BulkWriteOptions { BypassDocumentValidation = false }, cancellationToken);
        }

        public virtual async Task UpdateAsync(BsonValue k, UpdateDefinition<TEntity> update) {
            var filter = new BsonDocument("_id", k);
            await Collection.FindOneAndUpdateAsync(filter, update);
        }

        public virtual async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default) {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            await Collection.DeleteOneAsync(filter, cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) {
            var filter = Builders<TEntity>.Filter.Eq("_id", GetKey(entity));
            await Collection.DeleteOneAsync(filter, cancellationToken);
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            var entities = Query(predicate);
            var listWrites = new List<WriteModel<TEntity>>();
            foreach (var entity in entities) {
                listWrites.Add(new DeleteOneModel<TEntity>(Builders<TEntity>.Filter.Eq("_id", GetKey(entity))));
            }
            await Collection.BulkWriteAsync(listWrites, new BulkWriteOptions { BypassDocumentValidation = false }, cancellationToken);
        }
        #endregion

        public virtual IMongoCollection<TEntity> GetCollection(string name) {
            return _context.Database.GetCollection<TEntity>(name);
        }

        private object GetKey(TEntity entity) {
            var key = FieldsCache.GetOrAdd(typeof(TEntity), ParseEntityInfo).Fields.FirstOrDefault(x => x.Name.ToLower() == "id");
            if (key == null) throw new NotSupportedException();
            return key.GetValue(entity);
        }

        private UpdateDefinition<TEntity> BuildUpdate(TEntity entity) {
            UpdateDefinitionBuilder<TEntity> updateBuilder = Builders<TEntity>.Update;
            UpdateDefinition<TEntity> update = null;
            var fields = FieldsCache.GetOrAdd(typeof(TEntity), ParseEntityInfo).Fields.Where(x => x.Name.ToLower() != "id");
            foreach (var item in fields) {
                if (item.GetCustomAttribute<NotMappedAttribute>() != null) continue;

                update = update == null ?
                    updateBuilder.Set(new StringFieldDefinition<TEntity, object>(item.Name), item.GetValue(entity)) :
                    update.Set(new StringFieldDefinition<TEntity, object>(item.Name), item.GetValue(entity));
            }
            return update;
        }

        private EntityInfoCache ParseEntityInfo(Type type) {
            var cache = new EntityInfoCache {
                Fields = type.GetProperties().ToArray()
            };

            var attr = type.GetCustomAttribute<TableAttribute>();
            if (attr != null && !attr.Name.IsNullOrEmpty()) {
                cache.TableName = attr.Name;
            } else {
                cache.TableName = type.Name;
            }

            return cache;
        }

        sealed class EntityInfoCache { 
            public PropertyInfo[] Fields    { get; set; }
            public string         TableName { get; set; }
        }

        sealed class MongoAssistor<T,TK> {
            private const    string CollectionName = "EntityConfs";
            private readonly IMongoDatabase _mongoDatabase;
            private readonly IMongoCollection<EntityConf> _confCollection;
            public MongoAssistor(IMongoDatabase mongoDatabase) {
                _mongoDatabase  = mongoDatabase;
                _confCollection = _mongoDatabase.GetCollection<EntityConf>(CollectionName);
            }

            public TK IncreaseId() {
                var filter = Builders<EntityConf>.Filter.Eq(x => x.Name, typeof(T).Name);
                var update = Builders<EntityConf>.Update.Inc(x => x.Value, 1L);
                var entity = _confCollection.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<EntityConf, EntityConf> {
                    IsUpsert       = true,
                    ReturnDocument = ReturnDocument.After,
                });
                return (TK)Convert.ChangeType(entity.Value, typeof(TK));
            }
        }

        public class EntityConf {
            public ObjectId Id    { get; set; }
            public string   Name  { get; set; }
            public long     Value { get; set; }
        }
    }
}
