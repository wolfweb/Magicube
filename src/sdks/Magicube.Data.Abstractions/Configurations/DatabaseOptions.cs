using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Data.Abstractions {
    public class DatabaseOptions {
        private ConcurrentDictionary<Type, List<Type>> _entities;
        public DatabaseOptions() {
            _entities = new ConcurrentDictionary<Type, List<Type>>();
        }

        public IEnumerable<Type> Entities       => _entities.Keys;
        public IEnumerable<Type> EntityMappings => _entities.Values.Where(x => x != null).SelectMany(x => x);

        public string Name                      { get; set; }
        public string Value                     { get; set; }

        public DatabaseOptions RegisterEntity<T>() where T : IEntity {
            _entities.TryAdd(typeof(T), null);
            return this;
        }

        public DatabaseOptions RegisterEntityWithConfig<T, TConfig>()
            where T : IEntity
            where TConfig : class {
			_entities.AddOrUpdate(typeof(T), type => new List<Type> { typeof(TConfig) }, (type, list) => {
				var mapType = typeof(TConfig);
				if (!list.Contains(mapType)) list.Add(mapType);
				return list;
			});
			return this;
		}
	}
}
