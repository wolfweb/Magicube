using Magicube.Core.Reflection;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Magicube.Core {
    public static class New<T> {
        private static readonly ConcurrentDictionary<Type, Func<T>> _cache = new();

        public static readonly Func<T> Instance;

        public static readonly Func<Type, T> Creator = (type) => {
            var func = _cache.GetOrAdd(type, x => {
                if (type == typeof(string))
                    return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();
                else if (type.HasDefaultConstructor())
                    return Expression.Lambda<Func<T>>(Expression.New(type)).Compile();

                return () => (T)FormatterServices.GetUninitializedObject(type);
            });
            return func();
        };

        static New() { 
            Type type = typeof(T);
            if (type == typeof(string))
                Instance = Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();
            else if (type.HasDefaultConstructor())
                Instance = Expression.Lambda<Func<T>>(Expression.New(type)).Compile();
            else
                Instance = () => (T)FormatterServices.GetUninitializedObject(type);
        }
    }

    public static class New<T, TArg0> {
        public static readonly Func<TArg0, T> Instance;

        static New() {
            var service = TypeAccessor.Get<T>();
            var ctor = service.GetConstructor(typeof(TArg0));
            if (ctor == null) throw new MagicubeException(1000, $"无效的构造函数参数:{service.Context.FullName}");
            var p0 = Expression.Parameter(typeof(TArg0));
            Instance = Expression.Lambda<Func<TArg0, T>>(Expression.New(ctor.Constructor, p0), new[] { p0 }).Compile();
        }
    }

    public static class New<T, TArg0, TArg1> {
        public static readonly Func<TArg0, TArg1, T> Instance;

        static New() {
            var service = TypeAccessor.Get<T>();
            var ctor = service.GetConstructor(typeof(TArg0),typeof(TArg1));
            if (ctor == null) throw new MagicubeException(1000, $"无效的构造函数参数:{service.Context.FullName}");
            var p0 = Expression.Parameter(typeof(TArg0));
            var p1 = Expression.Parameter(typeof(TArg1));
            Instance = Expression.Lambda<Func<TArg0, TArg1, T>>(Expression.New(ctor.Constructor, p0, p1), new[] { p0, p1 }).Compile();
        }
    }

    public static class New<T, TArg0, TArg1, TArg2> {
        public static readonly Func<TArg0, TArg1, TArg2, T> Instance;

        static New() {
            var service = TypeAccessor.Get<T>();
            var ctor = service.GetConstructor(typeof(TArg0), typeof(TArg1),typeof(TArg2));
            if (ctor == null) throw new MagicubeException(1000, $"无效的构造函数参数:{service.Context.FullName}");
            var p0 = Expression.Parameter(typeof(TArg0));
            var p1 = Expression.Parameter(typeof(TArg1));
            var p2 = Expression.Parameter(typeof(TArg2));
            Instance = Expression.Lambda<Func<TArg0, TArg1, TArg2, T>>(Expression.New(ctor.Constructor, p0, p1, p2), new[] { p0, p1, p2 }).Compile();
        }
    }

    public static class New<T, TArg0, TArg1, TArg2,TArg3> {
        public static readonly Func<TArg0, TArg1, TArg2, TArg3, T> Instance;

        static New() {
            var service = TypeAccessor.Get<T>();
            var ctor = service.GetConstructor(typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3));
            if (ctor == null) throw new MagicubeException(1000, $"无效的构造函数参数:{service.Context.FullName}");
            var p0 = Expression.Parameter(typeof(TArg0));
            var p1 = Expression.Parameter(typeof(TArg1));
            var p2 = Expression.Parameter(typeof(TArg2));
            var p3 = Expression.Parameter(typeof(TArg3));
            Instance = Expression.Lambda<Func<TArg0, TArg1, TArg2, TArg3, T>>(Expression.New(ctor.Constructor, p0, p1, p2,p3), new[] { p0, p1, p2,p3 }).Compile();
        }
    }

    public static class New<T, TArg0, TArg1, TArg2, TArg3, TArg4> {
        public static readonly Func<TArg0, TArg1, TArg2, TArg3, TArg4, T> Instance;

        static New() {
            var service = TypeAccessor.Get<T>();
            var ctor = service.GetConstructor(typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4));
            if (ctor == null) throw new MagicubeException(1000, $"无效的构造函数参数:{service.Context.FullName}");
            var p0 = Expression.Parameter(typeof(TArg0));
            var p1 = Expression.Parameter(typeof(TArg1));
            var p2 = Expression.Parameter(typeof(TArg2));
            var p3 = Expression.Parameter(typeof(TArg3));
            var p4 = Expression.Parameter(typeof(TArg4));
            Instance = Expression.Lambda<Func<TArg0, TArg1, TArg2, TArg3, TArg4, T>>(Expression.New(ctor.Constructor, p0, p1, p2, p3, p4), new[] { p0, p1, p2, p3, p4 }).Compile();
        }
    }

    public static class New<T, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5> {
        public static readonly Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, T> Instance;

        static New() {
            var service = TypeAccessor.Get<T>();
            var ctor = service.GetConstructor(typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5));
            if (ctor == null) throw new MagicubeException(1000, $"无效的构造函数参数:{service.Context.FullName}");
            var p0 = Expression.Parameter(typeof(TArg0));
            var p1 = Expression.Parameter(typeof(TArg1));
            var p2 = Expression.Parameter(typeof(TArg2));
            var p3 = Expression.Parameter(typeof(TArg3));
            var p4 = Expression.Parameter(typeof(TArg4));
            var p5 = Expression.Parameter(typeof(TArg5));
            Instance = Expression.Lambda<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, T>>(Expression.New(ctor.Constructor, p0, p1, p2, p3, p4, p5), new[] { p0, p1, p2, p3, p4, p5 }).Compile();
        }
    }
}
