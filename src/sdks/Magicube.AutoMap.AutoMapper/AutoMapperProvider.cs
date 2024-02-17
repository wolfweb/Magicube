using AutoMapper;
using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.AutoMap.AutoMapper {
    public class AutoMapperProvider : IMapperProvider {
        public readonly IServiceScopeFactory _scopeFactory;

        public AutoMapperProvider(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
        }

        public TDest Map<TSourse, TDest>(TSourse src) {
            using (var scope = _scopeFactory.CreateScope())
                return scope.ServiceProvider.GetService<IMapper>().Map<TSourse, TDest>(src);
        }

        public TDest Map<TSourse, TDest>(TSourse src, TDest dest) {
            using (var scope = _scopeFactory.CreateScope())
                return scope.ServiceProvider.GetService<IMapper>().Map(src, dest);
        }

        public object Map(object source, Type src, Type dest) {
            using (var scope = _scopeFactory.CreateScope())
                return scope.ServiceProvider.GetService<IMapper>().Map(source, src, dest);
        }
    }
}
