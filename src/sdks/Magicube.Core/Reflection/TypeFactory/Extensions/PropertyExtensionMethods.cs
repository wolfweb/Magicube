using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Magicube.Core.Reflection.Builders;

namespace Magicube.Core.Reflection {
    public static class PropertyExtensionMethods {
        public static bool IsProperty(this MethodInfo methodInfo) {
            return methodInfo.IsPropertyGet() || methodInfo.IsPropertySet();
        }

        public static bool IsPropertyGet(this MethodInfo methodInfo) {
            return methodInfo != null && methodInfo.Name.StartsWith("get_");
        }

        public static bool IsPropertySet(this MethodInfo methodInfo) {
            return methodInfo != null && methodInfo.Name.StartsWith("set_");
        }

        public static string PropertyGetName(this MemberInfo memberInfo) {
            if (memberInfo != null && memberInfo is PropertyInfo) {
                return string.Format("get_{0}", memberInfo.Name);
            }

            return null;
        }

        public static string PropertySetName(this MemberInfo memberInfo) {
            if (memberInfo != null &&
                memberInfo is PropertyInfo) {
                return string.Format("set_{0}", memberInfo.Name);
            }

            return null;
        }

        public static PropertyBuilder Getter(this PropertyBuilder propertyBuilder, Func<MethodBuilder> action) {
            var method = action();
            propertyBuilder.SetGetMethod(method);
            return propertyBuilder;
        }

        public static PropertyBuilder Setter(this PropertyBuilder propertyBuilder, Func<MethodBuilder> action) {
            var method = action();
            propertyBuilder.SetSetMethod(method);
            return propertyBuilder;
        }

        public static IEmitter GetProperty(this IEmitter emitter, string propertyName, ILocal local) {
            return emitter
                .LdLocS(local)
                .GetProperty(propertyName, local.LocalType);
        }

        public static IEmitter GetProperty<TProperty>(this IEmitter emitter, string propertyName) {
            return emitter
                .GetProperty(propertyName, typeof(TProperty));
        }

        public static IEmitter GetProperty(this IEmitter emitter, string propertyName, Type propertyType) {
            MethodInfo getMethod = propertyType.GetProperty(propertyName).GetGetMethod();
            return emitter
                .CallVirt(getMethod);
        }

        public static IEmitter SetProperty(this IEmitter emitter, string propertyName, ILocal local) {
            return emitter
                .LdLocS(local)
                .SetProperty(propertyName, local.LocalType);
        }

        public static IEmitter SetProperty(this IEmitter emitter, string propertyName, Type propertyType) {
            var setMethod = propertyType.GetProperty(propertyName).GetSetMethod();
            return emitter
                .CallVirt(setMethod);
        }

        public static IEmitter SetProperty<TProperty>(this IEmitter emitter, string propertyName) {
            return emitter.SetProperty(propertyName, typeof(TProperty));
        }

        /// <summary>
        /// 适用于无参构造函数的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IPropertyBuilder SetCustomAttribute<T>(this IPropertyBuilder builder) where T : Attribute {
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
        public static IPropertyBuilder SetCustomAttribute<T>(this IPropertyBuilder builder, PropertyInfo[] namedProperties, object[] propertyValues) where T : Attribute {
            var typeAccessor = TypeAccessor.Get<T>().Context;
            var attr = new CustomAttributeBuilder(typeAccessor.Constructors.First().Constructor, Array.Empty<object>(), namedProperties, propertyValues);
            builder.SetCustomAttribute(attr);
            return builder;
        }
    }
}