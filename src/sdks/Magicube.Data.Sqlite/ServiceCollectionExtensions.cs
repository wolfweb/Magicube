using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Migration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Data.Sqlite {
    public static class ServiceCollectionExtensions {
        public const string DatabaseFile = "magicube.db";
        public const string Identity = "SQLite";

        public static IServiceCollection UseSqlite(this IServiceCollection services) {
            services.UseSqlite(null);
            return services;
        }

        public static IServiceCollection UseSqlite(this IServiceCollection services, DatabaseOptions options) {
            if (options != null) {
                services.Configure<DatabaseOptions>(x => {
                    x.Value             = options.Value;
                    x.Name              = Identity;
                });
            }

            AddSqliteCore(services);
            services.ConfigDatabase<SqliteDbContextProvider>(Identity, builder => {
                builder.UseSqlite(options?.Value)/*.UseLazyLoadingProxies()*/;
            });

            return services;
        }

        public static IServiceCollection AddSqlite(this IServiceCollection services) {
            AddSqliteCore(services);
            services.AddDatabase(new DatabaseProvider {
                Name                    = Identity,
                RequireConnection       = true,
                ExampleConnectionString = "Data Source=magicube.db"
            }).ConfigDatabase<SqliteDbContextProvider>(Identity);
            return services;
        }

        private static void AddSqliteCore(IServiceCollection services) {
            services.AddDatabaseCore()
                .AddEFCore()
                .AddTransient<SqliteCompiler>() 
                .AddSqLiteMigration(Identity)
                .AddTransient<ISqlBuilder, SqliteBuilder>();
        }
    }
}
