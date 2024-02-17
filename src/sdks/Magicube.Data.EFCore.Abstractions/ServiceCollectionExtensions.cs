using Magicube.Core;
using Magicube.Data.Abstractions.Loggings;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Magicube.Data.Abstractions.EfDbContext;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Magicube.Data.Abstractions.SqlBuilder.Operators;
using Magicube.Data.Abstractions.Mapping;

namespace Magicube.Data.Abstractions {
    public static class ServiceCollectionExtensions {
		public static IServiceCollection AddEFCore(this IServiceCollection services) {
			RegisterEntities(services);

			services.TryAddScoped<MagicubeDbContextFactory>();
			services.TryAddSingleton(typeof(DbOperator<>));
			services.TryAddSingleton<IEntityBuilder, DefaultEntityBuilder>();
			services.TryAddTransient(typeof(IRepository<,>), typeof(Repository<,>));
			services.TryAddTransient<IDynamicEntityRepository, DynamicEntityRepository>();

			return services;
		}

		public static IServiceCollection ConfigDatabase<T>(this IServiceCollection services, string keyName, Action<DbContextOptionsBuilder> configure = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) 
			where T : class, IMagicubeDbContextProvider {
			services.AddDbContext<IDbContext, MagicubeDbContext>(builder => {
				configure?.Invoke(builder);
				builder.EnableSensitiveDataLogging(true);
				builder.UseLoggerFactory(LoggerFactory.Create(x => x.AddProvider(new SqlProfileLoggerProvider())));
			}, contextLifetime).AddScoped<IMagicubeDbContextProvider, T>(keyName);

			services.TryAddScoped<IUnitOfWork>(x => x.GetService<IDbContext>() as MagicubeDbContext);
			return services;
		}

		public static IServiceCollection AddDatabseContext<TContext>(this IServiceCollection services, string identity, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : DefaultDbContext {
			services.TryAdd(new ServiceDescriptor(typeof(DbContextOptions<TContext>), (IServiceProvider p) => CreateDbContextOptions<TContext>(p, null), optionsLifetime));
			services.TryAdd(new ServiceDescriptor(typeof(DbContextOptions), (IServiceProvider p) => p.GetRequiredService<DbContextOptions<TContext>>(), optionsLifetime));
			services.Add(identity, typeof(IDbContext), typeof(TContext), contextLifetime);
			return services;
		}

		public static IServiceCollection AddEntity<T, TMapping>(this IServiceCollection services)
			where T : IEntity
			where TMapping : class, IMappingConfiguration {
			services.Configure<DatabaseOptions>(options => options.RegisterEntity<T, TMapping>());
			return services;
		}

		private static DbContextOptions<TContext> CreateDbContextOptions<TContext>(IServiceProvider applicationServiceProvider, Action<IServiceProvider, DbContextOptionsBuilder> optionsAction) where TContext : DbContext {
			DbContextOptionsBuilder<TContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<TContext>(new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));
			dbContextOptionsBuilder.UseApplicationServiceProvider(applicationServiceProvider);
			optionsAction?.Invoke(applicationServiceProvider, dbContextOptionsBuilder);
			return dbContextOptionsBuilder.Options;
		}

		private static void RegisterEntities(IServiceCollection services) {
			services.AddEntity<DbTable, DbTableMapping>();
		}
	}
}
