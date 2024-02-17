using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class SByteConverter : BaseTypeConverter<sbyte> {
        protected override void SerializeInternal(sbyte obj, SerializationStream stream) {
            stream.Write((byte)obj);
        }

        protected override sbyte DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadSByte();
        }

        public override SerializedType Type => SerializedType.Sbyte;
    }
}
