using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magicube.Data.Abstractions.Mapping {
    public abstract class EntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntity {
        public virtual void ApplyConfiguration(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfiguration(this);
        }

        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
