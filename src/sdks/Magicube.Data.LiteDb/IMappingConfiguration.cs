using LiteDB;
using Magicube.Data.Abstractions;

namespace Magicube.Data.LiteDb {
	public interface IMappingConfiguration {
		void ApplyConfiguration(BsonMapper mapper);
	}

	public abstract class EntityTypeConfiguration<TEntity> : IMappingConfiguration where TEntity : class, IEntity {
		public virtual void ApplyConfiguration(BsonMapper mapper) {
			var builder = mapper.Entity<TEntity>();
			Configure(builder);
		}

		public abstract void Configure(EntityBuilder<TEntity> builder);
	}
}