using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Microsoft.EntityFrameworkCore;

namespace Magicube.Data.Sqlite {
    public class SqliteDbContextProvider : IMagicubeDbContextProvider {
        public void Configure(DatabaseOptions options, DbContextOptionsBuilder builder) {
            builder.UseSqlite(options.Value);
        }
    }
}
