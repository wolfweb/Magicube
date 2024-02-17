using Magicube.Data.Abstractions.Mapping;

namespace Magicube.Data.Abstractions {
	public static class DatabaseOptionsExtension {
        public static DatabaseOptions RegisterEntity<T, TMapping>(this DatabaseOptions options)
			where T : IEntity
			where TMapping : class, IMappingConfiguration {
            options.RegisterEntityWithConfig<T, TMapping>();
			return options;
		}
	}
}
