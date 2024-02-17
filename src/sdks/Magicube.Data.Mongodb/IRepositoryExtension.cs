using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using Magicube.Data.Abstractions;
using System.Collections.Generic;

namespace Magicube.Data.Mongodb {
    public static class IRepositoryExtension {
        public static async Task<T> GetAsync<T, TKey, TProperty>(this IRepository<T, TKey> repository, TKey id, Expression<Func<T, TProperty>> propertyPath) where T : class, IEntity<TKey> {
            throw new NotImplementedException();
        }

        public static async Task<T> GetAsync<T, TKey, TProperty>(this IRepository<T, TKey> repository, Expression<Func<T, bool>> predicate, Expression<Func<T, TProperty>> propertyPath) where T : class, IEntity<TKey> {
            throw new NotImplementedException();
        }

        public static async Task<IList<T>> GetsAsync<T, TKey, TProperty>(this IRepository<T, TKey> repository, Expression<Func<T, bool>> predicate, Expression<Func<T, TProperty>> propertyPath) where T : class, IEntity<TKey> {
            throw new NotImplementedException();
        }
    }
}
