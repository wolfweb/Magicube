using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Magicube.Core {
	using Magicube.Core.Reflection;
#if !DEBUG
	using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class ObjectExtension {
        public static T ConvertTo<T>(this object v) {
            var type = typeof(T);
            return (T)ConvertTo(v, type);
        }

        public static object ConvertTo(this object v, Type type) {
            var realType = Nullable.GetUnderlyingType(type) ?? type;
            if(v == null && realType.IsGenericType) return Activator.CreateInstance(realType);

            if (realType.IsEnum) {
                if (v is string) return Enum.Parse(realType, v.ToString());
                return Enum.ToObject(realType, v);
            }

            if (!realType.IsInterface && realType.IsGenericType) {
                Type innerType = realType.GetGenericArguments()[0];
                object innerValue = v.ConvertTo(innerType);
                return Activator.CreateInstance(realType, new object[] { innerValue });
            }

            if (v is string && type == typeof(Guid)) return new Guid(v as string);
            if (v is string && type == typeof(Version)) return new Version(v as string);

            if (v is IEnumerable enumerable) {
                if (type is { IsGenericType: true }) {
                    var desiredCollectionItemType = type.GenericTypeArguments[0];
                    var desiredCollectionType = typeof(ICollection<>).MakeGenericType(desiredCollectionItemType);

                    if (type.IsAssignableFrom(desiredCollectionType)) {
                        var collectionType = typeof(List<>).MakeGenericType(desiredCollectionItemType);
                        var collection = New<IList>.Creator(collectionType);
                        foreach (var item in enumerable) {
                            var convertedItem = ConvertTo(item, desiredCollectionItemType);
                            collection.Add(convertedItem);
                        }

                        return collection;
                    }
                }
            }

            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(realType))
                return converter.ConvertFrom(v);

            if (typeof(IConvertible).IsAssignableFrom(realType))
                return Convert.ChangeType(v, realType);

            return v;
        }
    }
}
