using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions {
    public class DatabaseOptions {
        public ConcurrentDictionary<Type, List<Type>> EntityConfs { get; }
        public DatabaseOptions() {
            EntityConfs = new ConcurrentDictionary<Type, List<Type>>();
        }

        public string Name                      { get; set; }
        public string Value                     { get; set; }

        public DatabaseOptions RegisterEntity<T>() where T : IEntity {
            EntityConfs.TryAdd(typeof(T), null);
            return this;
        }

        public DatabaseOptions RegisterEntityWithConfig<T, TConfig>()
            where T : IEntity
            where TConfig : class {
			EntityConfs.AddOrUpdate(typeof(T), type => new List<Type> { typeof(TConfig) }, (type, list) => {
				var mapType = typeof(TConfig);
				if (!list.Contains(mapType)) list.Add(mapType);
				return list;
			});
			return this;
		}
	}
}
