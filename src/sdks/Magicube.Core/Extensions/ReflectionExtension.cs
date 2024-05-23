using Magicube.Core.Reflection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Magicube.Core {
#if !DEBUG
    using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class ReflectionExtension {
        private static ConcurrentDictionary<Type, object> ValueTypeDefaultValueMapping = new();
             
        private static NullabilityInfoContext _nullabilityInfoContext = new();

        public static Dictionary<string, object> GetPropertieValues<T>(this T obj) where T : class {
            var properties = TypeAccessor.Get<T>().Context.Properties.Select(x=>x.Member);
            return properties.Where(item => item.GetValue(obj, null) != null).ToDictionary(item => item.Name, item => item.GetValue(obj, null));
        }

        public static bool HasDefaultConstructor(this Type type) {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static Type GetBaseType(this Type type, Func<Type, bool> predicate) {
            do {
                if (predicate(type)) break;
                type = type.BaseType;
            } while (!type.Equals(typeof(object)));
            return type;
        }

        public static Type GetCollectionType(this Type type) {
            if( type.IsArray ) return type.GetElementType();
            var enumerableType = type.GetGenericEnumerableType();
            return enumerableType != null
                ? enumerableType.GetGenericArguments()[0]
                : typeof(object);
        }

        public static Type GetRealElementType(this Type type) {
            Type ienum = FindIEnumerable(type);
            if (ienum == null)  return type;

            return ienum.GetGenericArguments()[0];
        }

        public static Type GetGenericEnumerableType(this Type type) {
            return type.GetInterface(IsGenericEnumerableType);
        }

        public static Type GetInterface(this Type type, Predicate<Type> predicate) {
            if (predicate(type))
                return type;

            return Array.Find(type.GetInterfaces(), predicate);
        }

        public static MethodInfo GetMethodWithOptionalTypes(this Type type, string methodName, params Type[] argumentTypes) {
            if (argumentTypes != null && argumentTypes.Any() == true) {
                return type.GetMethod(methodName, argumentTypes);
            }

            return type.GetMethod(methodName);
        }

        public static Type UnwrapNullable(this Type type) {
            return type.IsNullable() ? type.GetGenericArguments()[0] : type;
        }

        public static bool IsCollection(this Type type) {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && type != typeof(string);
        }

        public static bool IsGenericEnumerableType(this Type type) {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool IsNullable(this Type type) {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullable(this PropertyInfo property) {
            var nullabilityInfo = _nullabilityInfoContext.Create(property);
            return nullabilityInfo.WriteState == NullabilityState.Nullable;
        }

        public static bool IsNullable(this FieldInfo field) {
            var nullabilityInfo = _nullabilityInfoContext.Create(field);
            return nullabilityInfo.WriteState == NullabilityState.Nullable;
        }

        public static void SetValue<T>(this T obj, string propertyOrField, object value) where T : class {
            var propertyInfo = TypeAccessor.Get<T>(obj.GetType(), obj).GetProperty(propertyOrField);
            if (propertyInfo != null) {
                propertyInfo.SetValue(obj, value.ConvertTo(propertyInfo.PropertyType));
            } else {
                var field = TypeAccessor.Get(obj.GetType(), obj).GetField(propertyOrField);
                if (field != null) { 
                    field.SetValue(obj, value.ConvertTo(field.FieldType));
                }
            }
        }

        public static void SetValue<T>(this T obj, FieldInfo field, object value) where T : class {
            SetValue(obj, field.Name, value);
        }

        public static void SetValue<T>(this T obj, PropertyInfo property, object value) where T : class {
            SetValue(obj, property.Name, value);
        }

        public static object GetValue<T>(this T obj, string propertyOrField) where T : class {
            var propertyInfo = TypeAccessor.Get<T>(obj.GetType(), obj).GetProperty(propertyOrField);
            if (propertyInfo != null) {
                return propertyInfo.GetValue(obj).ConvertTo(propertyInfo.PropertyType);
            } else {
                var field = TypeAccessor.Get(obj.GetType(), obj).GetField(propertyOrField);
                if (field != null) {
                    return field.GetValue(obj).ConvertTo(field.FieldType);
                }
            }
            return null;
        }

        public static object GetValue<T>(this T obj, FieldInfo field) where T : class {
            return GetValue(obj, field.Name);
        }

        public static object GetValue<T>(this T obj, PropertyInfo property) where T : class {
            return GetValue(obj, property.Name);
        }

        public static object GetValue(this Type type) {
            if (type.IsSimpleType()) {
                return ValueTypeDefaultValueMapping.GetOrAdd(type, x => {
                    if (x.IsEnum) return 0;

                    if (x == typeof(string)) return null;

                    return Activator.CreateInstance(x);
                });
            }
            return default;
        }

        public static bool IsSimpleType(this Type type)  {
            if(type.IsValueType || typeof(string) == type) {
                return true;
            }

            if (type.IsConstructedGenericType && type.Name.Equals("Nullable`1")) {
                return IsSimpleType(type.GenericTypeArguments.First());
            }
            return false;
        }

        public static T    GetAttribute<T>(this Type type, string fieldName) where T : Attribute {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            var attribute = t.GetField(fieldName)?.GetCustomAttribute<T>();
            return attribute;
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(this Type type) {
            if (type.IsSealed && type.IsAbstract && !type.IsGenericType && !type.IsNested) {
                var query = from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            where method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                            select method;
                return query;
            }

            return Enumerable.Empty<MethodInfo>();
        }

        public static bool IsInherited<T>(this Type type) {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool IsInheritedClass(this Type type) {
            if (typeof(IEnumerable).IsAssignableFrom(type)) return false;

            if (type.GetInterfaces().Any()) return true;
            return IsInheritedAbstractClass(type);   
        }

        public static bool IsInheritedAbstractClass(this Type type) {
            if (type.IsAbstract) return true;
            if (type != typeof(object)) {
                return IsInheritedAbstractClass(type.BaseType);
            }
            return false;
        }

        private static Type FindIEnumerable(Type type) {
            if (type == null || type == typeof(string))  return null;

            if (type.IsArray) {
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());
            }

            if (type.IsGenericType) {
                foreach (Type arg in type.GetGenericArguments()) {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);

                    if (ienum.IsAssignableFrom(type)) {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = type.GetInterfaces();

            if (ifaces != null && ifaces.Length > 0) {
                foreach (Type iface in ifaces) {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) {
                        return ienum;
                    }
                }
            }

            if (type.BaseType != null && type.BaseType != typeof(object)) {
                return FindIEnumerable(type.BaseType);
            }

            return null;
        }
    }
}
