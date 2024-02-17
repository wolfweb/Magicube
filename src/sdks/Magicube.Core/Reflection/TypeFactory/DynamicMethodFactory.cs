namespace Magicube.Core.Reflection {
    using System;
    using Magicube.Core.Reflection.Builders;
    public class DynamicMethodFactory {
        private static Lazy<DynamicMethodFactory> instance = new Lazy<DynamicMethodFactory>(() => new DynamicMethodFactory(), true);

        public static DynamicMethodFactory Default {
            get {
                return instance.Value;
            }
        }

        public IDynamicMethodBuilder NewDynamicMethod(string methodName, Type methodOwner) {
            return new FluentDynamicMethodBuilder(methodName, methodOwner);
        }
    }
}