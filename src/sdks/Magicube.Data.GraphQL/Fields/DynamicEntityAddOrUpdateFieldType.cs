using GraphQL;
using GraphQL.Types;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityAddOrUpdateFieldType : DynamicEntityFilterFieldType {
        const string WhereKey = "where";
        public DynamicEntityAddOrUpdateFieldType(DynamicEntity entity, ISchema schema) {
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

            if (where != null) {
                await rep.UpdateBatchAsync(entity, x => BuildWhereExpression(where, x));
                return null;
            } else {
                if (entity.Id > 0) {
                    await rep.UpdateAsync(entity);
                } else {
                    var id = await rep.InsertAsync(entity);
                    entity.Id = id;
                }

                return entity;
            }
        }
    }
}
