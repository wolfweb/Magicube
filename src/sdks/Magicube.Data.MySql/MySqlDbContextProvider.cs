using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Microsoft.EntityFrameworkCore;

namespace Magicube.Data.MySql {
    public class MySqlDbContextProvider : IMagicubeDbContextProvider {
        public void Configure(DatabaseOptions options, DbContextOptionsBuilder builder) {
            builder.UseMySql(options.Value, ServerVersion.AutoDetect(options.Value));
        }
    }
}
