using Magicube.Core;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Operators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Query = SqlKata.Query;

namespace Magicube.Data.Abstractions.EfDbContext {
    public static class DynamicEntityRepositoryExtension {
        public static async Task DeleteAsync(this IDynamicEntityRepository repository, string name, Func<OperatorContext<DeleteOperator>, OperatorContext<DeleteOperator>> queryFilter = null) {
            await repository.EnsureInitialize();
            var _sqlBuilder = repository.ServiceScope.ServiceProvider.GetService<ISqlBuilder>();
            var operatorCtx = _sqlBuilder.Delete(name);
            var whereCtx = queryFilter?.Invoke(operatorCtx);
            SqlResult rawSql;
            if (whereCtx != null) {
                rawSql = whereCtx.Build();
            } else {
                rawSql = operatorCtx.Build();
            }
            (repository.DbContext as DefaultDbContext).Execute(rawSql);
        }


        public static async Task<IList<DynamicEntity>> GetsAsync(
            this IDynamicEntityRepository repository, 
            string name, 
            Func<Query, Query> queryFilter = null, 
            int lastId = 0, 
            int size = 30
            ) {
            return await GetsAsync(repository, name, null, queryFilter, lastId, size);
        }
        public static async Task<IList<DynamicEntity>> GetsAsync(
            this IDynamicEntityRepository repository, 
            string name, 
            string[] columns, 
            Func<Query, Query> queryFilter = null, 
            int skip = 0, 
            int lastId = 0, 
            int size = 30) {

            await repository.EnsureInitialize();

            var _sqlBuilder = repository.ServiceScope.ServiceProvider.GetService<ISqlBuilder>();

            var table = DynamicEntityRepository.Tables.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (table == null) throw new DataException("unknow dynamic entity name");

            SqlKata.Query queryCtx = _sqlBuilder.Query(name);

            if (columns != null && columns.Any()) {
                queryCtx.Select(columns);
            }

            if (queryFilter != null) {
                queryCtx.Where(query => queryFilter.Invoke(query));
            }

            if (skip > 0)
                queryCtx.Skip(skip);
            else if (lastId > 0)
                queryCtx.WhereGt(Entity.IdKey, lastId);

            queryCtx.OrderByDesc(Entity.IdKey);

            if (size > 0) queryCtx.Limit(size);

            var rawSql = queryCtx.Build();

            var dataTable = (repository.DbContext as DefaultDbContext).SqlQuery(rawSql);
            var result = new List<DynamicEntity>();
            foreach (DataRow row in dataTable.Rows) {
                DynamicEntity entity = row;
                result.Add(entity);
            }
            return result;
        }
        public static async Task UpdateBatchAsync(this IDynamicEntityRepository repository, DynamicEntity entity, Func<OperatorContext<UpdateOperator>, OperatorContext<UpdateOperator>> queryFilter = null) {
            await repository.EnsureInitialize();

            var _sqlBuilder = repository.ServiceScope.ServiceProvider.GetService<ISqlBuilder>();
            var operatorCtx = _sqlBuilder.Update(entity.TableName);

            foreach (var field in entity.Fields) {
                if (field.Key == Entity.IdKey) continue;
                operatorCtx.Set(field.Key, entity[field.Key]);
            }

            var whereCtx = queryFilter?.Invoke(operatorCtx);
            SqlResult rawSql;
            if (whereCtx != null) {
                rawSql = whereCtx.Build();
            } else {
                rawSql = operatorCtx.Build();
            }

            (repository.DbContext as DefaultDbContext).Execute(rawSql);
        }
    }
}
