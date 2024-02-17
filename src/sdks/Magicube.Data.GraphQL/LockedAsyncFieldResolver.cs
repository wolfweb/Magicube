using System.Threading.Tasks;
using System;
using GraphQL;
using GraphQL.Resolvers;

namespace Magicube.Data.GraphQL {
    public class LockedAsyncFieldResolver<TReturnType> : IFieldResolver {
        private readonly Func<IResolveFieldContext, Task<TReturnType>> _resolver;

        public LockedAsyncFieldResolver(Func<IResolveFieldContext, Task<TReturnType>> resolver) {
            _resolver = resolver;
        }

        public async ValueTask<object> ResolveAsync(IResolveFieldContext context) {
            var graphContext = (GraphQLContext)context.UserContext;
            await graphContext.ExecutionContextLock.WaitAsync();
            try {
                return await _resolver(context);
            } finally {
                graphContext.ExecutionContextLock.Release();
            }
        }
    }
}
