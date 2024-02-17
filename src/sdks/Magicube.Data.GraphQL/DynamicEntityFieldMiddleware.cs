using System.Threading.Tasks;
using GraphQL.Instrumentation;
using GraphQL;
using Magicube.Data.Abstractions;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityFieldMiddleware : IFieldMiddleware {
        public ValueTask<object> ResolveAsync(IResolveFieldContext context, FieldMiddlewareDelegate next) {
            if (context.Source != null) {
                DynamicEntity result = context.Source as DynamicEntity;
                return new ValueTask<object>(result[context.FieldDefinition.Name]);
            }
            return next(context);
        }
    }
}
