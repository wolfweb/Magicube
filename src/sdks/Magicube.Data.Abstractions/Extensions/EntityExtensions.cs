using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace Magicube.Data.Abstractions {
    [DebuggerStepThrough]
    public static class EntityExtensions {
        private static JsonSerializer _jsonSerializer = new JsonSerializer { 
            TypeNameHandling = TypeNameHandling.Objects,
        };

        public static T As<T>(this IEntity entity) where T : class {
            var typeName = typeof(T).Name;
            return entity.As<T>(typeName);
        }

        public static T As<T>(this IEntity entity, string name) where T : class {
            JToken value;

            if (entity.Parts.TryGetValue(name, out value)) {
                return value.ToObject<T>(_jsonSerializer);
            }

            return default(T);
        }

        public static bool Has<T>(this IEntity entity) where T : class, new() {
            var typeName = typeof(T).Name;
            return entity.Has<T>(typeName);
        }

        public static bool Has<T>(this IEntity entity, string name) where T : class, new() {
            return entity.Parts[name] != null;
        }

        public static IEntity Put<T>(this IEntity entity, T aspect) where T : class {
            return entity.Put<T>(typeof(T).Name, aspect);
        }

        public static IEntity Put<T>(this IEntity entity, string name, object property) where T : class {
            entity.Parts[name] = JObject.FromObject(property, _jsonSerializer);
            return entity;
        }

        public static IEntity Alter<T>(this IEntity entity, string name, Action<T> action) where T : class {
            JToken value;
            T obj;

            if (!entity.Parts.TryGetValue(name, out value)) {
                obj = default(T);
            } else {
                obj = value.ToObject<T>(_jsonSerializer);
            }

            action?.Invoke(obj);

            entity.Put<T>(name, obj);

            return entity;
        }
    }
}
