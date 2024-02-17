using System;

namespace Magicube.Core.Reflection {
    public static class DynamicMethodExtensionMethods {
        public static Delegate Create(this IDynamicMethodBuilder dynamicMethodBuilder, Type delegateType) {
            return dynamicMethodBuilder.Define().CreateDelegate(delegateType);
        }

        public static Action CreateAction(this IDynamicMethodBuilder dynamicMethodBuilder) {
            return (Action)dynamicMethodBuilder.Define().CreateDelegate(typeof(Action));
        }

        public static Action<T> CreateAction<T>(this IDynamicMethodBuilder dynamicMethodBuilder) {
            return (Action<T>)dynamicMethodBuilder.Define().CreateDelegate(typeof(Action<T>));
        }

        public static Action<T1, T2> CreateAction<T1, T2>(this IDynamicMethodBuilder dynamicMethodBuilder) {
            return (Action<T1, T2>)dynamicMethodBuilder.Define().CreateDelegate(typeof(Action<T1, T2>));
        }

        public static Action<T1, T2, T3> CreateAction<T1, T2, T3>(this IDynamicMethodBuilder dynamicMethodBuilder) {
            return (Action<T1, T2, T3>)dynamicMethodBuilder.Define().CreateDelegate(typeof(Action<T1, T2, T3>));
        }

        public static Func<TReturn> CreateFunc<TReturn>(this IDynamicMethodBuilder dynamicMethodBuilder) {
            return (Func<TReturn>)dynamicMethodBuilder.Define().CreateDelegate(typeof(Func<TReturn>));
        }

        public static Func<T, TReturn> CreateFunc<T, TReturn>(this IDynamicMethodBuilder dynamicMethodBuilder) {
            return (Func<T, TReturn>)dynamicMethodBuilder.Define().CreateDelegate(typeof(Func<T, TReturn>));
        }

        public static Func<T1, T2, TReturn> CreateFunc<T1, T2, TReturn>(this IDynamicMethodBuilder dynamicMethodBuilder) {
            return (Func<T1, T2, TReturn>)dynamicMethodBuilder.Define().CreateDelegate(typeof(Func<T1, T2, TReturn>));
        }

        public static Func<T1, T2, T3, TReturn> CreateFunc<T1, T2, T3, TReturn>(this IDynamicMethodBuilder dynamicMethodBuilder) {
            return (Func<T1, T2, T3, TReturn>)dynamicMethodBuilder.Define().CreateDelegate(typeof(Func<T1, T2, T3, TReturn>));
        }
    }
}