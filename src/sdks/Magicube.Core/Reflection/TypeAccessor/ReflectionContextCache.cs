using System;
using System.Collections.Concurrent;

namespace Magicube.Core.Reflection {
    internal class ReflectionContextCache { 
        private static ConcurrentDictionary<Type, ReflectionContext> _reflectServiceCache = new();

        public static ReflectionContext GetOrAdd(Type type) {
            return _reflectServiceCache.GetOrAdd(type, x=>{
                return new ReflectionContext(x);
            });
        }
    }
}
