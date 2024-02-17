using Magicube.Core;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Data.LiteDb {
    public static class ServiceCollectionExtensions {
		public const string Identity = "LiteDb";

		public static IServiceCollection UseLiteDb(this IServiceCollection services) {
			services.UseLiteDb(null);
			return services;
		}

		public static IServiceCollection UseLiteDb(this IServiceCollection services, DatabaseOptions options) {
			if (options != null) {
				services.Configure<DatabaseOptions>(x => {
					x.Value = options.Value;
				});
			}
			AddLiteDbCore(services);
			services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
			return services;
		}

		public static IServiceCollection AddEntity<T, TMapping>(this IServiceCollection services)
			where T : IEntity
			where TMapping : class, IMappingConfiguration {
			services.Configure<DatabaseOptions>(options => options.RegisterEntity<T, TMapping>());
			return services;
		}

		public static IServiceCollection AddLiteDb(this IServiceCollection services) {
			AddLiteDbCore(services);
			services.AddDatabase(new DatabaseProvider { Name = Identity, RequireConnection = true, ExampleConnectionString = "magicube.db" });
			return services;
		}

		private static void AddLiteDbCore(IServiceCollection services) {
			services
				.AddSingleton<IDbContext, LiteDbContext>()
				.AddSingleton(x => x.GetService<IDbContext>() as ILiteDbContext)
				.AddScoped<IMigrationManager, NullMigrationManager>()
				;
		}
	}
}
