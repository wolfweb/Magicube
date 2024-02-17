using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter
{
    internal class UShortConverter : BaseTypeConverter<ushort>
    {
        protected override void SerializeInternal(ushort obj, SerializationStream stream) {
            byte[] data = BitConverter.GetBytes(obj);
            stream.Write(data);
        }

        protected override ushort DeserializeInternal(DeserializationStream stream, Type sourceType)
        {
            return stream.ReadUShort();
        }

        public override SerializedType Type => SerializedType.UShort;
    }
}
