using LiteDB;
using Magicube.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Data.LiteDb {
	public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Entity<TKey> {
		private readonly ILiteDbContext _context;
		private ILiteCollection<TEntity> _entities;

		public Repository(ILiteDbContext dbContext) {
			_context = dbContext;
		}

		public IQueryable<TEntity> All => _context.Set<TEntity, TKey>();

		public void Delete(TKey id) {
			Entities.Delete(new BsonValue(id));
		}

		public void Delete(TEntity entity) {
			Entities.Delete(new BsonValue(entity.Id));
		}

		public void Delete(Expression<Func<TEntity, bool>> predicate) {
			Entities.DeleteMany(predicate);
		}

		public Task DeleteAsync(TKey id, CancellationToken cancellationToken = default) {
			Delete(id);
			return Task.CompletedTask;
		}

		public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) {
			Delete(entity);
			return Task.CompletedTask;
		}

		public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
			Delete(predicate);
			return Task.CompletedTask;
		}

		public TEntity Get(TKey id) {
			return Entities.FindById(new BsonValue(id));
		}

		public TEntity Get(Expression<Func<TEntity, bool>> predicate) {
			return Entities.FindOne(predicate);
		}

		public ValueTask<TEntity> GetAsync(TKey id) {
			var result = Get(id);
			return ValueTask.FromResult(result);
		}

		public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate) {
			var result = Get(predicate);
			return Task.FromResult(result);
		}

		public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> predicate) {
			return Entities.Find(predicate);
		}

		public Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> predicate) {
			var result = Query(predicate);
			return Task.FromResult(result);
		}

		public TEntity Insert(TEntity entity) {
			var key = Entities.Insert(entity);
			//todo: 确认id
			return entity;
		}

		public void Insert(IEnumerable<TEntity> entities) {
			Entities.Insert(entities);
		}

		public Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) {
			var result = Insert(entity);
			return Task.FromResult(result);
		}

		public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
			Insert(entities);
			return Task.CompletedTask;
		}

		public TEntity Update(TEntity entity) {
			Entities.Update(entity);
			return entity;
		}

		public void Update(IEnumerable<TEntity> entities) {
			Entities.Update(entities);
		}

		public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) {
			var result = Update(entity);
			return Task.FromResult(result);
		}

		public Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
			Update(entities);
			return Task.CompletedTask;
		}

		protected virtual ILiteCollection<TEntity> Entities {
			get {
				if (_entities == null)
					_entities = _context.Collection<TEntity>();

				return _entities;
			}
		}
	}
}