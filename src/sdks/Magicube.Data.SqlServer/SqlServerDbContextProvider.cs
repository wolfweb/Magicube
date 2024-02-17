using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Microsoft.EntityFrameworkCore;

namespace Magicube.Data.SqlServer {
    public class SqlServerDbContextProvider : IMagicubeDbContextProvider {
        public void Configure(DatabaseOptions options, DbContextOptionsBuilder builder) {
            builder.UseSqlServer(options?.Value);
        }
    }
}
