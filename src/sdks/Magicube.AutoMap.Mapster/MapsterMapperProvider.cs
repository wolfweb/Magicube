using Magicube.Core;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.AutoMap.Mapster {
    public class MapsterMapperProvider : IMapperProvider {
        public readonly IServiceScopeFactory _serviceScopeFactory;

        public MapsterMapperProvider(IServiceScopeFactory serviceScopeFactory) {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public TDest Map<TSourse, TDest>(TSourse src) {
            using (var scope = _serviceScopeFactory.CreateScope())
                return scope.ServiceProvider.GetService<IMapper>().Map<TDest>(src);
        }

        public TDest Map<TSourse, TDest>(TSourse src, TDest dest) {
            using (var scope = _serviceScopeFactory.CreateScope())
                return scope.ServiceProvider.GetService<IMapper>().Map(src, dest);
        }

        public object Map(object source, Type src, Type dest) {
            using (var scope = _serviceScopeFactory.CreateScope())
                return scope.ServiceProvider.GetService<IMapper>().Map(source, src, dest);
        }
    }
}
