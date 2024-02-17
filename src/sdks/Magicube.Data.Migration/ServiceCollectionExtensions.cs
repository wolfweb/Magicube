using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Magicube.Core;
using Magicube.Core.Modular;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;
using System.Threading;

namespace Magicube.Data.Migration {
    public static class ServiceCollectionExtensions {
        private static volatile int _migrated = 0;
        public static IServiceCollection AddMySqlMigration(this IServiceCollection services, string identity = null) {
            AddMigration(identity, services, x => x.AddMySql5());
            return services;
        }

        public static IServiceCollection AddSqlServerMigration(this IServiceCollection services, string identity = null) {
            AddMigration(identity, services, x => x.AddSqlServer());          
            return services;
        }

        public static IServiceCollection AddSqLiteMigration(this IServiceCollection services, string identity = null) {
            AddMigration(identity, services, x => x.AddSQLite());          
            return services;
        }

        public static IServiceCollection AddPostgreSqlMigration(this IServiceCollection services, string identity = null) {
            AddMigration(identity, services,x => x.AddPostgres());
            return services;
        }

        public static IServiceCollection AddMigrationAssembly(this IServiceCollection services, params Assembly[] assemblies) {
            services.AddSingleton<IAssemblySourceItem>(new AssemblySourceItem(assemblies));
            return services;            
        }

        private static void AddMigration(string identity, IServiceCollection services, Action<IMigrationRunnerBuilder> configure) {
			services.Configure<ModularOptions>(x => x.ModularShareAssembly.Add(typeof(IMigrationManager).Assembly))
                .Configure<MigrationOption>(x => { });            

            if (!identity.IsNullOrEmpty()) {
                services.TryAddScoped<IMigrationManager, MigrationManager>(identity);
            } else {
                services.TryAddScoped<IMigrationManager, MigrationManager>();
            }

            if (Interlocked.CompareExchange(ref _migrated, 1, 0) == 0) {
                services.AddFluentMigratorCore();
            }

            services.Replace<IProcessorAccessor, MigrationProcessorAccessor>();
            services.Replace<IConnectionStringAccessor, MigrationConnectionProvider>();

            services.ConfigureRunner(x => {
                x.ScanIn(typeof(DefaultMigration).Assembly).For.Migrations();
                configure(x);
            });
        }
    }
}
