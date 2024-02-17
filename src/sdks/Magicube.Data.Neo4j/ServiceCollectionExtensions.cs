using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Data.Neo4j {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddNeo4j(this IServiceCollection services) {
            services.AddSingleton<INeo4jDbContext, Neo4jDbContext>()
                .AddTransient(typeof(INeo4jRepository<,>), typeof(Neo4jRepository<,>));

            return services;
        }
    }
}
