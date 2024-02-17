using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Migration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Data.SqlServer {
    public static class ServiceCollectionExtensions {
        public const string Identity = "SqlServer";

        public static IServiceCollection UseSqlServer(this IServiceCollection services) {
            services.UseSqlServer(null);
            return services;
        }

        public static IServiceCollection UseSqlServer(this IServiceCollection services, DatabaseOptions options) {
            if (options != null) { 
                services.Configure<DatabaseOptions>(x => {
                    x.Value             = options.Value;
                    x.Name              = Identity;
                });            
            }

            AddSqlServerCore(services);
            services.ConfigDatabase<SqlServerDbContextProvider>(Identity, builder => {
                builder.UseSqlServer(options?.Value)/*.UseLazyLoadingProxies()*/;
            });

            return services;
        }

        public static IServiceCollection AddSqlServer(this IServiceCollection services) {
            AddSqlServerCore(services);
            services.AddDatabase(new DatabaseProvider { 
                ExampleConnectionString = "Server=localhost;Database=magicubedb;User Id=username;Password=password", 
                Name                    = Identity, 
                RequireConnection       = true 
            }).ConfigDatabase<SqlServerDbContextProvider>(Identity);
            return services;
        }

        private static void AddSqlServerCore(IServiceCollection services) {
            services.AddDatabaseCore()
                .AddEFCore()
				.AddTransient<SqlServerCompiler>()
                .AddSqlServerMigration(Identity)
                .AddTransient<ISqlBuilder, SqlServerBuilder>();
        }
    }
}
