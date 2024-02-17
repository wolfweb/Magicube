using Magicube.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Data.VectorDb.Milvus {
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> {
        private readonly MilvusProvider _milvusProvider;

        public Repository(MilvusProvider milvusProvider) {
            _milvusProvider = milvusProvider;
        }

        public IQueryable<TEntity> All => throw new NotImplementedException();

        public void Delete(TKey id) {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity) {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TKey id, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public TEntity Get(TKey id) {
            throw new NotImplementedException();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate) {
            throw new NotImplementedException();
        }

        public ValueTask<TEntity> GetAsync(TKey id) {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate) {
            throw new NotImplementedException();
        }

        public TEntity Insert(TEntity entity) {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<TEntity> entities) {
            throw new NotImplementedException();
        }

        public Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> predicate) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> predicate) {
            throw new NotImplementedException();
        }

        public TEntity Update(TEntity entity) {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<TEntity> entities) {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }
    }
}
