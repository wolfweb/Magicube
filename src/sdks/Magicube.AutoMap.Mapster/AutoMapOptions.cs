using Mapster;
using Newtonsoft.Json.Linq;
using System;

namespace Magicube.AutoMap.Mapster {
    public class AutoMapOptions {
        private readonly TypeAdapterConfig _config;

        public TypeAdapterConfig Config => _config;

        public AutoMapOptions() {
            _config = new TypeAdapterConfig();
            _config.Default.IgnoreMember((model,site) => typeof(JToken).IsAssignableFrom(model.Type));
        }

        public void AddAutoMap<Source, Target>(Action<TypeAdapterSetter<Source, Target>> callback) {
            callback(_config.NewConfig<Source, Target>());            
        }
    }
}
