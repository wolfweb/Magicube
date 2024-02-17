using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Magicube.Data.Abstractions {
	public static class ServiceCollectionExtensions {
        public static IServiceCollection AddDatabaseCore(this IServiceCollection services) {
            services.Configure<DynamicDataSourceOptions>(x => { });
            services.TryAddScoped<RepositoryFactory>();
			services.TryAddScoped<IMigrationManager, NullMigrationManager>();
			services.TryAddScoped<IMigrationManagerFactory, MigrationManagerFactory>();
			return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, DatabaseProvider databaseProvider) {
            services.AddSingleton(databaseProvider);
            return services;
        }

        public static IServiceCollection AddEntity<T>(this IServiceCollection services) where T : IEntity {
            services.Configure<DatabaseOptions>(options => options.RegisterEntity<T>());
            return services;
        }
    }
}
