using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Magicube.Data.Abstractions.EfDbContext {
    public static class IRepositoryExtension {
        public static async Task<T> GetAsync<T, TKey, TProperty>(this IRepository<T, TKey> repository, TKey id, Expression<Func<T, TProperty>> propertyPath) where T : class, IEntity<TKey> {
            var dbSet = repository.All as DbSet<T>;
            return await dbSet.Include(propertyPath).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public static async Task<T> GetAsync<T, TKey, TProperty>(this IRepository<T, TKey> repository, Expression<Func<T, bool>> predicate, Expression<Func<T, TProperty>> propertyPath) where T : class, IEntity<TKey> {
            var dbSet = repository.All as DbSet<T>;
            return await dbSet.Include(propertyPath).SingleOrDefaultAsync(predicate);
        }

        public static async Task<IList<T>> GetsAsync<T, TKey, TProperty>(this IRepository<T, TKey> repository, Expression<Func<T, bool>> predicate, Expression<Func<T, TProperty>> propertyPath) where T : class, IEntity<TKey> {
            var dbSet = repository.All as DbSet<T>;
            return await dbSet.Where(predicate).Include(propertyPath).ToListAsync();
        }
        
        public static async Task<T> GetAsync<T, TKey>(this IRepository<T, TKey> repository, TKey id, Func<DbSet<T>, DbSet<T>> handler) where T : class, IEntity<TKey> {
            var dbSet = repository.All as DbSet<T>;
            return await handler(dbSet).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public static async Task<T> GetAsync<T, TKey>(this IRepository<T, TKey> repository, Expression<Func<T, bool>> predicate, Func<DbSet<T>, DbSet<T>> handler) where T : class, IEntity<TKey> {
            var dbSet = repository.All as DbSet<T>;
            return await handler(dbSet).SingleOrDefaultAsync(predicate);
        }

        public static async Task<IList<T>> GetsAsync<T, TKey>(this IRepository<T, TKey> repository, Func<DbSet<T>, IQueryable<T>> handler) where T : class, IEntity<TKey> {
            var dbSet = repository.All as DbSet<T>;
            return await handler(dbSet).ToListAsync();
        }
    }
}
