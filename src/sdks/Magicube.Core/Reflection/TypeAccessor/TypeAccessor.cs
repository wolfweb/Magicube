using System;
using System.Diagnostics.CodeAnalysis;

namespace Magicube.Core.Reflection {
    public static class TypeAccessor {
        public static ReflectService<T> Get<T>() {
            return Get<T>(typeof(T), default);
        }

        public static ReflectService<T> Get<T>(T obj) {
            return Get<T>(typeof(T), obj);
        }

        public static ReflectService<T> Get<T>([NotNull] Type type, T obj) {
            var ctx = ReflectionContextCache.GetOrAdd(type);
            return new ReflectService<T>(ctx, obj);
        }

        public static ReflectService<object> Get([NotNull] Type type, object obj) {
            var ctx = ReflectionContextCache.GetOrAdd(type);
            return new ReflectService<object>(ctx, obj);
        }
    }
}
