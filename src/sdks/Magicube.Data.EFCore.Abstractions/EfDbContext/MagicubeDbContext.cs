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
        private IDbContextTransaction _transaction;
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

        public void BeginTransaction() {
            if (_transaction == null)
                _transaction = Database.BeginTransaction();
        }

        public void Commit() {
            try {
                _transaction?.Commit();
                _transaction?.Dispose();
            } finally {
                _transaction = null;
            }
        }

        public void Rollback() {
            try {
                _transaction?.Rollback();
                _transaction?.Dispose();
            } finally {
                _transaction = null;
            }
        }
    }
}
