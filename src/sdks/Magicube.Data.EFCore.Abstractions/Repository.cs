using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Data.Abstractions {
    public class Repository<TEntity,TKey> : IRepository<TEntity,TKey> where TEntity : class, IEntity<TKey> {
        private DbSet<TEntity> _entities;
        private readonly IDbContext _context;
        public Repository(IDbContext context) {
            _context       = context;
        }

        #region Sync
        public virtual void           Delete(TEntity entity) {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Remove(entity);
            _context.SaveChanges();
        }

        public virtual void           Delete(TKey id) {
            var entity = Entities.Find(id);
            if (entity == null)
                throw new ArgumentOutOfRangeException(nameof(id));

            Entities.Remove(entity);
            _context.SaveChanges();
        }

        public virtual void           Delete(Expression<Func<TEntity, bool>> predicate) {
            var list = Entities.Where(predicate).ToArray();
            Entities.RemoveRange(list);
            _context.SaveChanges();
        }

        public virtual TEntity        Get(TKey id) {
            return Entities.Find(id);
        }

        public virtual TEntity        Get(Expression<Func<TEntity, bool>> predicate) {
            return Entities.Where(predicate).FirstOrDefault();
        }

        public virtual IEnumerable<TEntity>  Query(Expression<Func<TEntity, bool>> predicate) {
            return Entities.Where(predicate).ToArray();
        }

        public virtual TEntity        Insert(TEntity entity) {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var res = Entities.Add(entity);
            _context.SaveChanges();
            return res.Entity;
        }

        public virtual void           Insert(IEnumerable<TEntity> entities) {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.AddRange(entities);
            _context.SaveChanges();
        }

        public virtual TEntity        Update(TEntity entity) {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var res = Entities.Update(entity);
            _context.SaveChanges();
            return res.Entity;
        }

        public virtual void           Update(IEnumerable<TEntity> entities) {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.UpdateRange(entities);
            _context.SaveChanges();
        }
        #endregion

        #region Async
        public virtual async Task                 DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task                 DeleteAsync(TKey id, CancellationToken cancellationToken = default) {
            var entity = Entities.Find(id);
            if (entity == null)
                throw new ArgumentOutOfRangeException(nameof(id));

            Entities.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);            
        }

        public virtual async Task                 DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            var list = Entities.Where(predicate).ToArray();
            Entities.RemoveRange(list);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
        }

        public virtual async ValueTask<TEntity>   GetAsync(TKey id) {
            return await Entities.FindAsync(id);
        }

        public virtual async Task<TEntity>        GetAsync(Expression<Func<TEntity, bool>> predicate) {
            return await Entities.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TEntity>>  QueryAsync(Expression<Func<TEntity, bool>> predicate) {
            return await Entities.Where(predicate).ToArrayAsync();
        }

        public virtual async Task<TEntity>        InsertAsync(TEntity entity, CancellationToken cancellationToken = default) {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var res = await Entities.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return res.Entity;
        }

        public virtual async Task                 InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await Entities.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<TEntity>        UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var res = Entities.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return res.Entity;
        }

        public virtual async Task                 UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.UpdateRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
        #endregion

        protected virtual DbSet<TEntity> Entities {
            get {
                if (_entities == null)
                    _entities = _context.Set<TEntity, TKey>() as DbSet<TEntity>;

                return _entities;
            }
        }

        IQueryable<TEntity> IRepository<TEntity,TKey>.All => Entities;
    }
}
