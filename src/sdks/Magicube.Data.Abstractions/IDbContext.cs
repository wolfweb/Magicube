using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Data.Abstractions {
    public interface IDbContext {
        IQueryable<TEntity> Set<TEntity, TKey>() where TEntity : class, IEntity<TKey>;
        IQueryable<TEntity> Set<TEntity>() where TEntity : class, IEntity;

        int                 SaveChanges();
        Task<int>           SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
