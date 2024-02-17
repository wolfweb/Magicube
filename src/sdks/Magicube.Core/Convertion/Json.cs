using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;

namespace Magicube.Core {
    public static class Json {
        static JsonSerializerSettings JsonSerializerSetting = new JsonSerializerSettings {
            NullValueHandling    = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            DateFormatHandling   = DateFormatHandling.IsoDateFormat,
#if DEBUG
            Formatting           = Formatting.Indented
#else                            
            Formatting           = Formatting.None,
#endif
        };

        static ConcurrentDictionary<string, JsonSerializerSettings> _settings = new ConcurrentDictionary<string, JsonSerializerSettings>();

        public const string DefaultSettingKey = nameof(DefaultSettingKey);

        public static JsonSerializerSettings CreateSettings(string key, bool useCamelCase = false, bool keepTypeObject = false) {
            return _settings.GetOrAdd(key, k => 
                 new JsonSerializerSettings { 
                    NullValueHandling    = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    DateFormatHandling   = DateFormatHandling.IsoDateFormat,
                    ContractResolver     = useCamelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver(),
                    TypeNameHandling     = keepTypeObject ? TypeNameHandling.Objects : TypeNameHandling.None
                }
            );
        }

        public static T Parse<T>(string json, JsonSerializerSettings settings = null) {
            return Parse<T>(json, typeof(T), settings);
        }
        public static T Parse<T>(string json, Type type, JsonSerializerSettings settings = null) {
            if (typeof(T).IsInheritedClass()) {
                settings = CreateSettings(DefaultSettingKey, keepTypeObject: true);
                if (json.Contains("$type")) {
                    var obj = JsonConvert.DeserializeObject<JObject>(json);
                    var jsonRealType = Type.GetType(obj.GetValue("$type").ToString());
                    return (T)JsonConvert.DeserializeObject(json, jsonRealType, settings);
                }
            }
            return (T)Parse(json, type, settings);
        }
        public static object Parse(string json, Type type, JsonSerializerSettings settings = null) {
            if (json.Contains("$type")) settings = CreateSettings(DefaultSettingKey, keepTypeObject: true);
            return JsonConvert.DeserializeObject(json, type, settings ?? JsonSerializerSetting);
        }

        public static string Stringify<T>(T model, JsonSerializerSettings settings = null) {
            if (typeof(T).IsInheritedClass()) settings = CreateSettings(DefaultSettingKey, keepTypeObject: true);
            return JsonConvert.SerializeObject(model, settings ?? JsonSerializerSetting);
        }
    }
}
