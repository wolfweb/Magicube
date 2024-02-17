using Microsoft.EntityFrameworkCore;

namespace Magicube.Data.Abstractions.EfDbContext
{
    public interface IMagicubeDbContextProvider {
        void Configure(DatabaseOptions options, DbContextOptionsBuilder builder);
    }
}
