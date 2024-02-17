using Magicube.Core;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Data.Mongodb {
    public static class ServiceCollectionExtensions {
        public const string Identity = "Mongodb";

        public static IServiceCollection UseMongodb(this IServiceCollection services) {
            services.UseMongodb(null);
            return services;
        }

        public static IServiceCollection UseMongodb(this IServiceCollection services, DatabaseOptions options) {
            if (options != null) {
                services.Configure<DatabaseOptions>(x => {
                    x.Value             = options.Value;
                    x.Name              = Identity;
                });
            }
            AddMongoCore(services);
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            return services;
        }        

        public static IServiceCollection AddMongodb(this IServiceCollection services) {
            AddMongoCore(services);
            services.AddDatabase(new DatabaseProvider { Name = Identity, RequireConnection = true, ExampleConnectionString = "mongodb://localhost:27017/magicubedb" });
            return services;
        }

        private static void AddMongoCore(IServiceCollection services) {
            services
                .AddSingleton<MongoProvider>()
                .AddScoped<IMongoDbContext, MongoDbContext>()
                .AddTransient(typeof(IMongoRepository<,>), typeof(Repository<,>))
                .AddScoped<IMigrationManager, NullMigrationManager>()
                ;
        }
    }
}
