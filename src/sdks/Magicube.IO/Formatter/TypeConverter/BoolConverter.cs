using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class BoolConverter : BaseTypeConverter<bool> {
        protected override void SerializeInternal(bool obj, SerializationStream stream) {
            byte[] data = BitConverter.GetBytes(obj);
            stream.Write(data);
        }

        protected override bool DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadBool();
        }

        public override SerializedType Type => SerializedType.Bool;
    }
}
