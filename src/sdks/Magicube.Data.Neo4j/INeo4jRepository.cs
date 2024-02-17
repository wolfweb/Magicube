using Magicube.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Magicube.Data.Neo4j {
    public interface INeo4jRepository<TEntity, TKey> where TEntity : Entity<TKey> {
        Task<IEnumerable<TEntity>> All();
        Task Add(TEntity item);
        Task Delete(Expression<Func<TEntity, bool>> query);
        Task DeleteRelationship<TEntity2, TRelationship>(Expression<Func<TEntity, bool>> query1, Expression<Func<TEntity2, bool>> query2, TRelationship relationship) 
            where TEntity2 : Entity<TKey>
            where TRelationship : Relationship;

        Task<IEnumerable<KeyValuePair<TEntity2, TRelationship>>> GetRelated<TEntity2, TRelationship>(Expression<Func<TEntity, bool>> query1, TRelationship relationship = null)
            where TEntity2 : Entity<TKey>
            where TRelationship : Relationship;

        Task Patch(Expression<Func<TEntity, bool>> query, TEntity item);

        Task Relate<TEntity2, TRelationship>(Expression<Func<TEntity, bool>> query1, Expression<Func<TEntity2, bool>> query2, TRelationship relationship)
            where TEntity2 : Entity<TKey>
            where TRelationship : Relationship;

        Task<TEntity> Single(Expression<Func<TEntity, bool>> query);

        Task Update(Expression<Func<TEntity, bool>> query, TEntity newItem);

        Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> query);


    }
}
