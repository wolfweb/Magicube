using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class IntConverter : BaseTypeConverter<int> {
        protected override void SerializeInternal(int obj, SerializationStream stream) {
            byte[] data = BitConverter.GetBytes(obj);
            stream.Write(data);
        }

        protected override int DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadInt();
        }
        public override SerializedType Type => SerializedType.Int;
    }
}
