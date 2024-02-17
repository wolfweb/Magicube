using System;
using System.IO;

namespace Magicube.IO {
    public static class ObjectExtension {
        public static T Clone<T>(this T source) where T : class {
            if (!typeof(T).IsInterface && !typeof(T).IsSerializable) {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

            if (ReferenceEquals(source, null)) {
                return default(T);
            }

            var formatter = new BinaryConverter();
            Stream stream = new MemoryStream();
            using (stream) {
                var bytes = formatter.Serialize(source);
                return formatter.Deserialize<T>(bytes);
            }
        }
    }
}
