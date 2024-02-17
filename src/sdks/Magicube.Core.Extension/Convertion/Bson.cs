using MessagePack;
using MessagePack.Resolvers;

namespace Magicube.Core.Convertion {
    public static class Bson {
        private static MessagePackSerializerOptions _options = new MessagePackSerializerOptions(TypelessObjectResolver.Instance);
        public static byte[] Serialize<T>(T model) {
            return MessagePackSerializer.Serialize(model, _options);
        }

        public static T Parse<T>(byte[] value) {
            return MessagePackSerializer.Deserialize<T>(value, _options);
        }

        public static string ToJson(byte[] value) { 
            return MessagePackSerializer.ConvertToJson(value, _options);
        }
    }
}
