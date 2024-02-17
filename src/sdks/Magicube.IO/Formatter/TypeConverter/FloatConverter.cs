using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class FloatConverter : BaseTypeConverter<float> {
        protected override void SerializeInternal(float obj, SerializationStream stream) {
            byte[] data = BitConverter.GetBytes(obj);
            stream.Write(data);
        }

        protected override float DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadFloat();
        }

        public override SerializedType Type => SerializedType.Float;
    }
}
