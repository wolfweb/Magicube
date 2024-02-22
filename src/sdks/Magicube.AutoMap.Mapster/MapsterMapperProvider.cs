using Magicube.Core;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.AutoMap.Mapster {
    public class MapsterMapperProvider : IMapperProvider {
        public readonly Application _app;

        public MapsterMapperProvider(Application app) {
            _app = app;
        }

        public TDest Map<TSourse, TDest>(TSourse src) {
            using (var scope = _app.CreateScope())
                return scope.GetService<IMapper>().Map<TDest>(src);
        }

        public TDest Map<TSourse, TDest>(TSourse src, TDest dest) {
            using (var scope = _app.CreateScope())
                return scope.GetService<IMapper>().Map(src, dest);
        }

        public object Map(object source, Type src, Type dest) {
            using (var scope = _app.CreateScope())
                return scope.GetService<IMapper>().Map(source, src, dest);
        }
    }
}
