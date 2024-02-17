using GraphQL.Types;
using System.Threading.Tasks;
using GraphQL;
using Newtonsoft.Json.Linq;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Magicube.Data.Abstractions.EfDbContext;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityDeleteFieldType : DynamicEntityFilterFieldType {
        const string WhereKey = "where";
        public DynamicEntityDeleteFieldType(DynamicEntity entity, ISchema schema) {
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

            if (context.HasArgument(WhereKey)) {
                where = JObject.FromObject(context.Arguments[WhereKey].Value);
            }

            await rep.DeleteAsync(tbName, x => BuildWhereExpression(where, x));
            return null;
        }
    }
}
