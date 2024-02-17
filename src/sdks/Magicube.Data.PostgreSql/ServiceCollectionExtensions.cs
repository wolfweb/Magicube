using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Migration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Data.PostgreSql {
    public static class ServiceCollectionExtensions {
        public const string Identity = "PostgreSQL";
        public static IServiceCollection UsePostgreSql(this IServiceCollection services) {
            services.UsePostgreSql(null);
            return services;
        }

        public static IServiceCollection UsePostgreSql(this IServiceCollection services, DatabaseOptions options) {
            if (options != null) {
                services.Configure<DatabaseOptions>(x => {
                    x.Value             = options.Value;
                    x.Name              = Identity;
                });
            }

            AddPostgreSqlCore(services);
            services.ConfigDatabase<PostgreSqlDbContextProvider>(Identity, builder => {
                builder.UseNpgsql(options?.Value)/*.UseLazyLoadingProxies()*/;
            });

            return services;
        }        

        public static IServiceCollection AddPostgreSql(this IServiceCollection services) {
            AddPostgreSqlCore(services);
            services.AddDatabase(new DatabaseProvider { 
                ExampleConnectionString = "Server=localhost;Port=5432;Database=magicubedb;User Id=username;Password=password", 
                RequireConnection       = true, 
                Name                    = Identity 
            }).ConfigDatabase<PostgreSqlDbContextProvider>(Identity);
            return services;
        }

        private static void AddPostgreSqlCore(IServiceCollection services) {
            services.AddDatabaseCore()
				.AddEFCore()
				.AddTransient<PostgresCompiler>()
                .AddPostgreSqlMigration(Identity) 
                .AddTransient<ISqlBuilder, PostgreSqlBuilder>();
        }
    }
}
