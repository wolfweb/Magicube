using AutoMapper;
using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.AutoMap.AutoMapper {
    public class AutoMapperProvider : IMapperProvider {
        public readonly Application _app;

        public AutoMapperProvider(Application app) {
            _app = app;
        }

        public TDest Map<TSourse, TDest>(TSourse src) {
            using (var scope = _app.CreateScope())
                return scope.GetService<IMapper>().Map<TSourse, TDest>(src);
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
