using Magicube.Data.Mongodb;
using Magicube.Data.MySql;
using Magicube.Data.PostgreSql;
using Magicube.Data.Sqlite;
using Magicube.Data.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Data.DbFactroy {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddDatabases(this IServiceCollection services) {
            services
                .AddMySQL()
                .AddSqlite()
                .AddSqlServer()
                .AddPostgreSql()
                .AddMongodb();

            return services;
        }
    }
}
