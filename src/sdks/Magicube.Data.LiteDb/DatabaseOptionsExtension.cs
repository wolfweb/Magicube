using Magicube.Data.Abstractions;

namespace Magicube.Data.LiteDb {
	public static class DatabaseOptionsExtension {
		public static DatabaseOptions RegisterEntity<T, TMapping>(this DatabaseOptions options)
			where T : IEntity
			where TMapping : class, IMappingConfiguration {
			options.RegisterEntityWithConfig<T, TMapping>();
			return options;
		}
	}
}