using Magicube.Data.Abstractions;
using Milvus.Client;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Magicube.Core;
using Magicube.Core.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections;

namespace Magicube.Data.VectorDb.Milvus {
    public interface IMilvusDbContext : IDbContext {
        MilvusClient Database { get; }
    }

    public class MilvusDbContext : IMilvusDbContext {
        public MilvusDbContext(MilvusProvider milvusProvider) {
            Database = milvusProvider.Database;
        }

        public MilvusClient Database { get; }

        public int SaveChanges() => 1;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

        IQueryable<TEntity> IDbContext.Set<TEntity, TKey>() {
            var accessor = TypeAccessor.Get<TEntity>();
            var attr = accessor.Context.Attributes.OfType<TableAttribute>().FirstOrDefault();
            string tableName;
            if (attr != null && !attr.Name.IsNullOrEmpty()) {
                tableName = attr.Name;
            }
            else {
                tableName = accessor.Context.Name;
            }

            var collection = Database.GetCollection(tableName);

            //Database.GetCollection(tableName).SearchAsync;

            throw new System.NotImplementedException();
        }

        IQueryable<TEntity> IDbContext.Set<TEntity>() {
            throw new System.NotImplementedException();
        }


        sealed class MilvusQueryable<TEntity> : IQueryable<TEntity> {
            public MilvusQueryable(MilvusQueryProvider queryProvider) {
                Provider   = queryProvider;
                Expression = Expression.Constant(this);
            }

            public MilvusQueryable(MilvusQueryProvider queryProvider, Expression expression) {
                Provider   = queryProvider;
                Expression = expression;
            }

            public Type           ElementType => typeof(TEntity);
            public Expression     Expression  { get; private set; }
            public IQueryProvider Provider    { get; private set; }

            public IEnumerator<TEntity> GetEnumerator() {
                return (Provider.Execute<IEnumerable<TEntity>>(Expression)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
            }
        }

        public abstract class MilvusQueryProvider : IQueryProvider {
            public IQueryable CreateQuery(Expression expression) {
                Type elementType = expression.Type.GetRealElementType();
                try {
                    return (IQueryable)Activator.CreateInstance(
                        typeof(MilvusQueryable<>).MakeGenericType(elementType),
                        new object[] { this, expression });
                }
                catch {
                    throw new Exception();
                }

            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
                return new MilvusQueryable<TElement>(this, expression);
            }

            object IQueryProvider.Execute(Expression expression) {
                return Execute(expression);
            }

            TResult IQueryProvider.Execute<TResult>(Expression expression) {
                return (TResult)Execute(expression);
            }

            public abstract object Execute(Expression expression);
        }
    }
}
