using GraphQL;
using GraphQL.NewtonsoftJson;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Data.GraphQL {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddGraphQL(this IServiceCollection services) {
            services.AddDatabaseCore();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>()
                .AddSingleton<IGraphQLSerializer, GraphQLSerializer>()
                .AddSingleton<ISchemaBuilder, DynamicEntitySchemaQuery>()
                .AddSingleton<IGraphQLProvider, GraphQLProvider>()
                .AddTransient<GraphQLOperateBuilder>();

            return services;
        }
    }
}
