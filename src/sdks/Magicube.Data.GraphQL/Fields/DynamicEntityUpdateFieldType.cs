using GraphQL.Types;
using System.Threading.Tasks;
using GraphQL;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Magicube.Data.Abstractions.EfDbContext;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityUpdateFieldType : DynamicEntityFilterFieldType {
        const string WhereKey = "where";
        public DynamicEntityUpdateFieldType(DynamicEntity entity, ISchema schema) {
            Type = typeof(DynamicEntityType);

            var whereInput = new DynamicEntityWhere(entity);

            Arguments = new QueryArguments(
                new QueryArgument<NonNullGraphType<DynamicEntityInput>> { Name = entity.TableName, ResolvedType = new DynamicEntityInput(entity) },
                new QueryArgument<DynamicEntityWhere> { Name = WhereKey, Description = "filters the dynamic entity items", ResolvedType = whereInput }
                );

            Resolver = new LockedAsyncFieldResolver<DynamicEntity>(Resolve);

            schema.RegisterType(whereInput);
        }

        private async Task<DynamicEntity> Resolve(IResolveFieldContext context) {
            var graphContext = (GraphQLContext)context.UserContext;

            var rep = graphContext.ServiceProvider.GetService<IDynamicEntityRepository>();
            JObject where = null;
            var tbName = context.Arguments.Keys.FirstOrDefault();

            var entity = await rep.NewAsync(tbName);

            var datas = context.Arguments[tbName].Value as Dictionary<string, object>;

            foreach (var item in datas) {
                entity[item.Key] = item.Value;
            }

            if (context.HasArgument(WhereKey)) {
                where = JObject.FromObject(context.Arguments[WhereKey].Value);
            }

            if (where == null && entity.Id == 0) throw new DataException("更新需要指定where条件或指定id");

            if (where != null) {
                await rep.UpdateBatchAsync(entity, x => BuildWhereExpression(where, x));
                return null;
            } else {
                await rep.UpdateAsync(entity);
                return entity;
            }
        }
    }
}
