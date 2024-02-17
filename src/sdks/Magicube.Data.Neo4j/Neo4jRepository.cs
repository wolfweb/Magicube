using Magicube.Core;
using Magicube.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Magicube.Data.Neo4j {
    public class Neo4jRepository<TEntity, TKey> : INeo4jRepository<TEntity, TKey> where TEntity : Entity<TKey> {
        private readonly INeo4jDbContext _dbContext;
        public Neo4jRepository(INeo4jDbContext dbContext) {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            _dbContext = dbContext;
        }

        public virtual async Task<IEnumerable<TEntity>> All() => await _dbContext.Client.Cypher
            .Match($"(x:{typeof(TEntity).Name})")
            .Return(x => x.As<TEntity>())
            .ResultsAsync;

        public virtual async Task Add(TEntity item) {
            await _dbContext.Client.Cypher
                    .Create($"(x:{typeof(TEntity).Name} {{item}})")
                    .WithParam("item", item)
                    .ExecuteWithoutResultsAsync();
        }

        public virtual async Task Delete(Expression<Func<TEntity, bool>> query) {
            string name = query.Parameters[0].Name;

            await _dbContext.Client.Cypher
                .Match($"({name}:{typeof(TEntity).Name})")
                .Where(query)
                .Delete(name)
                .ExecuteWithoutResultsAsync();
        }

        public virtual async Task DeleteRelationship<TEntity2, TRelationship>(Expression<Func<TEntity, bool>> query1, Expression<Func<TEntity2, bool>> query2, TRelationship relationship)
            where TEntity2 : Entity<TKey>
            where TRelationship : Relationship {
            string name1 = query1.Parameters[0].Name;
            string name2 = query2.Parameters[0].Name;

            await _dbContext.Client.Cypher
                .Match($"({name1}:{typeof(TEntity).Name})-[r:{relationship.Name}]->({name2}:{typeof(TEntity2).Name})")
                .Where(query1)
                .AndWhere(query2)
                .Delete("r")
                .ExecuteWithoutResultsAsync();
        }

        public virtual async Task<IEnumerable<KeyValuePair<TEntity2, TRelationship>>> GetRelated<TEntity2, TRelationship>(Expression<Func<TEntity, bool>> query1, TRelationship relationship = null)
            where TEntity2 : Entity<TKey>
            where TRelationship : Relationship {
            string name1 = query1.Parameters[0].Name;

            var datas = await _dbContext.Client.Cypher
                .Match($"({name1}:{typeof(TEntity).Name})-[r{(relationship != null ? $":{relationship.Name}" : "")}]->(rel:{typeof(TEntity2).Name})")
                .Where(query1)
                .Return((rel, r) => new {
                    k = rel.As<TEntity2>(),
                    v = r.Type()
                }).ResultsAsync;
            return datas.Select(x => {
                var r = New<TRelationship>.Instance();
                r.Name = x.v;
                return new KeyValuePair<TEntity2, TRelationship>(x.k, r);
            });
        }

        public virtual async Task Patch(Expression<Func<TEntity, bool>> query, TEntity item) {
            string name = query.Parameters[0].Name;

            await _dbContext.Client.Cypher
               .Match($"({name}:{typeof(TEntity).Name})")
               .Where(query)
               .Set($"{name} = {{item}}")
               .WithParam("item", item)
               .ExecuteWithoutResultsAsync();
        }

        public virtual async Task Relate<TEntity2, TRelationship>(Expression<Func<TEntity, bool>> query1, Expression<Func<TEntity2, bool>> query2, TRelationship relationship)
            where TEntity2 : Entity<TKey>
            where TRelationship : Relationship {
            string name1 = query1.Parameters[0].Name; 
            string name2 = query2.Parameters[0].Name;

            await _dbContext.Client.Cypher
                .Match($"({name1}:{typeof(TEntity).Name})", $"({name2}:{typeof(TEntity2).Name})")
                .Where(query1)
                .AndWhere(query2)
                .Merge($"({name1})-[:{relationship.Name}]->({name2})")
                .ExecuteWithoutResultsAsync();
        }

        public virtual async Task<TEntity> Single(Expression<Func<TEntity, bool>> query) {
            IEnumerable<TEntity> results = await Where(query);
            return results.FirstOrDefault();
        }

        public virtual async Task Update(Expression<Func<TEntity, bool>> query, TEntity newItem) {
            string name = query.Parameters[0].Name;

            await _dbContext.Client.Cypher
               .Match($"({name}:{typeof(TEntity).Name})")
               .Where(query)
               .Set($"{name} = {{item}}")
               .WithParam("item", newItem)
               .ExecuteWithoutResultsAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> query) {
            var newQuery = PredicateRewriter.Rewrite(query, "x");
            return await _dbContext.Client.Cypher.Match($"(x:{typeof(TEntity).Name})")
                .Where(newQuery)
                .Return(x => x.As<TEntity>())
                .ResultsAsync;
        }

        sealed class PredicateRewriter {
            public static Expression<Func<T, bool>> Rewrite<T>(Expression<Func<T, bool>> exp, string newParamName) {
                ParameterExpression param = Expression.Parameter(exp.Parameters[0].Type, newParamName);
                Expression newExpression  = new PredicateRewriterVisitor(param).Visit(exp);

                return (Expression<Func<T, bool>>)newExpression;
            }

            private class PredicateRewriterVisitor : ExpressionVisitor {
                private readonly ParameterExpression _parameterExpression;

                public PredicateRewriterVisitor(ParameterExpression parameterExpression) {
                    _parameterExpression = parameterExpression;
                }

                protected override Expression VisitParameter(ParameterExpression node) {
                    return _parameterExpression;
                }
            }
        }
    }
}
