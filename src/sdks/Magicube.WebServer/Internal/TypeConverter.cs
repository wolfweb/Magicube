using System;
using System.Collections.Concurrent;

namespace Magicube.WebServer.Internal {
    public static class TypeConverter {
        public static readonly ConcurrentDictionary<Type, object> GetDefaultValueCache = new ConcurrentDictionary<Type, object>();

        public static object ConvertType(object value, Type type) {
            if (value == DBNull.Value)
                value = null;

            if (value == null && type.IsValueType)
                return GetDefaultValue(type);

            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (underlyingType.IsEnum) {
                try {
                    value = Enum.Parse(underlyingType, value.ToString(), true);
                } catch (Exception exception) {
                    throw new TypeConversionException(String.Format("An error occurred while attempting to convert the value '{0}' to an enum of type '{1}'", value, underlyingType), exception);
                }
            }

            try {
                if (underlyingType == typeof(Guid)) {
                    if (value is string)
                        value = new Guid(value as string);
                    else if (value is byte[])
                        value = new Guid(value as byte[]);
                }

                object result = Convert.ChangeType(value, underlyingType);
                return result;
            } catch (Exception exception) {
                throw new TypeConversionException(String.Format("An error occurred while attempting to convert the value '{0}' to type '{1}'", value, underlyingType), exception);
            }
        }

        public static object GetDefaultValue(Type type) {
            return type.IsValueType ? GetDefaultValueCache.GetOrAdd(type, Activator.CreateInstance) : null;
        }

        public static int ToInt(this object obj) {
            return (int)obj;
        }

        [Serializable]
        public class TypeConversionException : Exception {
            public TypeConversionException(string message, Exception innerException)
                : base(message, innerException) {
            }
        }
    }
}
