using GraphQL.Types;
using System.Threading.Tasks;
using GraphQL;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityCreateFieldType : DynamicEntityFilterFieldType {
        public DynamicEntityCreateFieldType(DynamicEntity entity, ISchema schema) {
            Type = typeof(DynamicEntityType);

            var input = new DynamicEntityInput(entity);

            Arguments = new QueryArguments(
                new QueryArgument<NonNullGraphType<DynamicEntityInput>> { Name = entity.TableName, ResolvedType = new DynamicEntityInput(entity) }
                );

            Resolver = new LockedAsyncFieldResolver<DynamicEntity>(Resolve);

            schema.RegisterType(input);
        }

        private async Task<DynamicEntity> Resolve(IResolveFieldContext context) {
            var graphContext = (GraphQLContext)context.UserContext;

            var rep = graphContext.ServiceProvider.GetService<IDynamicEntityRepository>();
            var tbName = context.Arguments.Keys.FirstOrDefault();

            var entity = await rep.NewAsync(tbName);

            var datas = context.Arguments[tbName].Value as Dictionary<string, object>;

            if (datas == null) throw new DataException("没有有效的创建数据");

            foreach (var item in datas) {
                entity[item.Key] = item.Value;
            }

            var id = await rep.InsertAsync(entity);
            entity.Id = id;
            return entity;
        }
    }
}
