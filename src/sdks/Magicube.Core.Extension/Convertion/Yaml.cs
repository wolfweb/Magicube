using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Magicube.Core.Convertion {
    public static class Yaml {
        static ISerializer _serializer;
        static IDeserializer _deserializer;

        static Yaml() {
            _serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            _deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public static string Stringify<T>(T model) {
            return _serializer.Serialize(model);
        }

        public static T Parse<T>(string yaml) {
            yaml.NotNullOrEmpty();
            return _deserializer.Deserialize<T>(yaml);
        }
    }
}
