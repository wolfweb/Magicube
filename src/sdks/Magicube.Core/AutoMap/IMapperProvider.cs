using Magicube.Core.Reflection;
using System;

namespace Magicube.Core {
    public interface IMapperProvider {
        TDest Map<TSourse, TDest>(TSourse src);
        TDest Map<TSourse, TDest>(TSourse src, TDest dest);
        object Map(object source, Type src, Type dest);
    }

    public class MapperProvider : IMapperProvider {
        public TDest Map<TSourse, TDest>(TSourse src) {
            var result = New<TDest>.Instance();
            TypeAccessor.Get<TSourse>(src).Copy(result);
            return result;
        }

        public TDest Map<TSourse, TDest>(TSourse src, TDest dest) {
            TypeAccessor.Get<TSourse>(src).Copy(dest);
            return dest;
        }

        public object Map(object source, Type src, Type dest) {
            var result = Activator.CreateInstance(dest);
            TypeAccessor.Get(source).Copy(result);
            return result;
        }
    }
}
