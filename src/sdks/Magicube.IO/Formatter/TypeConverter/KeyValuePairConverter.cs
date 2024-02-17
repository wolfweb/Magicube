using System;
using Magicube.IO.Types;
using Magicube.IO.Utils;
using System.Collections.Generic;
using Magicube.IO.Streams;

namespace Magicube.IO.TypeConverter {
    internal class KeyValuePairConverter : BaseTypeConverter<object> {
        protected override void SerializeInternal(object obj, SerializationStream stream) {
            BinaryConverter converter = new BinaryConverter();
            KeyValuePair<object, object> objAsKeyValuePair = TypeHelper.CastFrom(obj);

            byte[] dataKey = converter.Serialize(objAsKeyValuePair.Key);
            stream.WriteWithLengthPrefix(dataKey);

            byte[] dataValue = converter.Serialize(objAsKeyValuePair.Value);
            stream.WriteWithLengthPrefix(dataValue);
        }

        protected override object DeserializeInternal(DeserializationStream stream, Type sourceType) {
            BinaryConverter converter = new BinaryConverter();

            byte[] keyData = stream.ReadBytesWithSizePrefix();
            var deserializedKey = converter.Deserialize<object>(keyData);

            byte[] valueData = stream.ReadBytesWithSizePrefix();
            var deserializedValue = converter.Deserialize<object>(valueData);

            var newKeyValuePair = Activator.CreateInstance(sourceType, deserializedKey, deserializedValue);
            return newKeyValuePair;
        }

        public override SerializedType Type => SerializedType.KeyValuePair;
    }
}
