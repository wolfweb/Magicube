using AutoMapper;
using Magicube.Core;
using System;
using System.Collections.Generic;

namespace Magicube.AutoMap.AutoMapper {
    public class AutoMapOptions {
        private readonly IDictionary<Type, Type> _auotMapModels;
        private readonly IList<Type> _autoMapProfile;
        public AutoMapOptions() {
            _auotMapModels = new Dictionary<Type, Type>();
            _autoMapProfile = new List<Type>();
        }

        public IDictionary<Type, Type> AuotMapModels => _auotMapModels;
        public IList<Type> AutoProfiles => _autoMapProfile;

        public AutoMapOptions RegisteModelMap<Source, Target>() {
            var sourceType = typeof(Source);
            if (_auotMapModels.ContainsKey(sourceType)) throw new MagicubeException(10000, $"");
            _auotMapModels.Add(sourceType, typeof(Target));

            return this;
        }

        public AutoMapOptions RegisteProfile<T>() where T : IProfileConfiguration {
            var type = typeof(T);
            if (_autoMapProfile.Contains(type)) throw new MagicubeException(10000, $"");
            _autoMapProfile.Add(type);
            return this;
        }
    }
}
