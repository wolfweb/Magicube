using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Migration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Data.MySql {
    public static class ServiceCollectionExtensions {
        public const string Identity = "MySql";

        public static IServiceCollection UseMySQL(this IServiceCollection services) {
            services.UseMySQL(null);
            return services;
        }

        public static IServiceCollection UseMySQL(this IServiceCollection services, DatabaseOptions options) {
            if (options != null ) { 
                services.Configure<DatabaseOptions>(x => {
                    x.Value             = options.Value;
                    x.Name              = Identity;
                });
            }

            AddMySqlCore(services);
            services.AddDatabaseCore()
                .ConfigDatabase<MySqlDbContextProvider>(Identity, builder => {
                builder.UseMySql(options?.Value, ServerVersion.AutoDetect(options.Value))/*.UseLazyLoadingProxies()*/;
            });

            return services;
        }

        public static IServiceCollection AddMySQL(this IServiceCollection services) {
            AddMySqlCore(services);
            services.AddDatabase(new DatabaseProvider { 
                Name                    = Identity, 
                IsDefault               = true, 
                RequireConnection       = true, 
                ExampleConnectionString = "Server=localhost;Database=magicubedb;Uid=username;Pwd=password" 
            }).ConfigDatabase<MySqlDbContextProvider>(Identity);
            return services;
        }

        private static void AddMySqlCore(IServiceCollection services) {
            services.AddDatabaseCore()
				.AddEFCore()
				.AddTransient<MySqlCompiler>()
                .AddMySqlMigration(Identity)
                .AddTransient<ISqlBuilder, MySqlBuilder>();
        }
    }
}
