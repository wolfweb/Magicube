using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace Magicube.Data.Abstractions.EfDbContext {
    public abstract class DefaultDbContext : DbContext, IDbContext {
        public DefaultDbContext(DbContextOptions options) : base(options) { }

        IQueryable<TEntity> IDbContext.Set<TEntity, TKey>() {
            return base.Set<TEntity>();
        }

        IQueryable<TEntity> IDbContext.Set<TEntity>() {
            return base.Set<TEntity>();
        }
    }

    public class MagicubeDbContext : DefaultDbContext, IUnitOfWork {
        private readonly IEntityBuilder _entityBuilder;
        private readonly MagicubeDbContextFactory _magicubeDbContextFactory;
        public MagicubeDbContext(
            DbContextOptions<MagicubeDbContext> options,
            IEntityBuilder entityBuilder,
            MagicubeDbContextFactory magicubeDbContextFactory
            ) : base(options) {
            _entityBuilder = entityBuilder;
            _magicubeDbContextFactory = magicubeDbContextFactory;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            _entityBuilder.Build(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            _magicubeDbContextFactory.Configure(optionsBuilder);
        }

        public IUnitOfWorkScoped BeginTransaction(){
            return new UnitOfWorkScoped(Database.BeginTransaction());
        }

        sealed class UnitOfWorkScoped : IUnitOfWorkScoped {
            private IDbContextTransaction _dbContextTransaction;

            public UnitOfWorkScoped(IDbContextTransaction dbContextTransaction) {
                _dbContextTransaction = dbContextTransaction;
            }

            public void Dispose() {
                _dbContextTransaction.Commit();
                _dbContextTransaction.Dispose();
            }

            public void Rollback() {
                _dbContextTransaction.Rollback();
            }
        }
    }
}
