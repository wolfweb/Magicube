using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Magicube.Core.Reflection.Emitters;

namespace Magicube.Core.Reflection {
    public static class MethodBuilderExtensionMethods {
        public static IMethodBuilder Public(this IMethodBuilder builder) {
            builder.Attributes |= MethodAttributes.Public;
            return builder;
        }

        public static IMethodBuilder Private(this IMethodBuilder builder) {
            builder.Attributes |= MethodAttributes.Private;
            return builder;
        }

        public static IMethodBuilder Virtual(this IMethodBuilder builder) {
            builder.Attributes |= MethodAttributes.Virtual;
            return builder;
        }

        public static IMethodBuilder HideBySig(this IMethodBuilder builder) {
            builder.Attributes |= MethodAttributes.HideBySig;
            return builder;
        }

        public static IMethodBuilder SpecialName(this IMethodBuilder builder) {
            builder.Attributes |= MethodAttributes.SpecialName;
            return builder;
        }

        public static IMethodBuilder RTSpecialName(this IMethodBuilder builder) {
            builder.Attributes |= MethodAttributes.RTSpecialName;
            return builder;
        }

        public static IMethodBuilder NewSlot(this IMethodBuilder builder) {
            builder.Attributes |= MethodAttributes.NewSlot;
            return builder;
        }

        public static IMethodBuilder Static(this IMethodBuilder builder) {
            builder.Attributes |= MethodAttributes.Static;
            return builder;
        }

        public static IMethodBuilder Param<TParam>(this IMethodBuilder methodBuilder) {
            return methodBuilder.Param<TParam>(null);
        }

        public static IMethodBuilder Param<TParam>(this IMethodBuilder methodBuilder, string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            return methodBuilder.Param(typeof(TParam), parameterName, attrs);
        }

        public static IMethodBuilder OutParam<TParam>(this IMethodBuilder methodBuilder, string parameterName) {
            return methodBuilder.Param(typeof(TParam).MakeByRefType(), parameterName, ParameterAttributes.Out);
        }

        public static IMethodBuilder RefParam<TParam>(this IMethodBuilder methodBuilder, string parameterName) {
            return methodBuilder.Param(typeof(TParam).MakeByRefType(), parameterName, ParameterAttributes.None);
        }

        public static IEmitter Body(this MethodBuilder methodBuilder) {
            var emitter = new ILGeneratorEmitter(methodBuilder.GetILGenerator());
            return emitter;
        }

        /// <summary>
        /// 适用于无参构造函数的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMethodBuilder SetCustomAttribute<T>(this IMethodBuilder builder) where T : Attribute {
            var typeAccessor = TypeAccessor.Get<T>().Context;
            var attr = new CustomAttributeBuilder(typeAccessor.Constructors.First().Constructor, Array.Empty<object>());
            builder.SetCustomAttribute(attr);
            return builder;
        }

        /// <summary>
        /// 适用于无参构造函数的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="namedProperties"></param>
        /// <param name="propertyValues"></param>
        /// <returns></returns>
        public static IMethodBuilder SetCustomAttribute<T>(this IMethodBuilder builder, PropertyInfo[] namedProperties, object[] propertyValues) where T : Attribute {
            var typeAccessor = TypeAccessor.Get<T>().Context;
            var attr = new CustomAttributeBuilder(typeAccessor.Constructors.First().Constructor, Array.Empty<object>(), namedProperties, propertyValues);
            builder.SetCustomAttribute(attr);
            return builder;
        }
    }
}