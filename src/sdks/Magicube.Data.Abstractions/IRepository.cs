using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Data.Abstractions {
    public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> {
        #region Sync
        void                       Delete(TKey id);
        void                       Delete(TEntity entity);
        void                       Delete(Expression<Func<TEntity, bool>> predicate);
        TEntity                    Get(TKey id);
        TEntity                    Get(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity>       Query(Expression<Func<TEntity, bool>> predicate);
        TEntity                    Insert(TEntity entity);
        void                       Insert(IEnumerable<TEntity> entities);
        TEntity                    Update(TEntity entity);
        void                       Update(IEnumerable<TEntity> entities);
        #endregion

        #region Async
        Task                       DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        Task                       DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task                       DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        ValueTask<TEntity>         GetAsync(TKey id);
        Task<TEntity>              GetAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity>              InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task                       InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<TEntity>              UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task                       UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        #endregion

        IQueryable<TEntity>        All { get; }
    }
}
