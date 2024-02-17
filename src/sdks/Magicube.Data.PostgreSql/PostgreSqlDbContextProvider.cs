using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Microsoft.EntityFrameworkCore;

namespace Magicube.Data.PostgreSql {
    public class PostgreSqlDbContextProvider : IMagicubeDbContextProvider {
        public void Configure(DatabaseOptions options, DbContextOptionsBuilder builder) {
            builder.UseNpgsql(options.Value);
        }
    }
}
