using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class CharConverter : BaseTypeConverter<char> {
        protected override void SerializeInternal(char obj, SerializationStream stream) {
            byte[] data = BitConverter.GetBytes(obj);
            stream.Write(data);
        }

        protected override char DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadChar();
        }

        public override SerializedType Type => SerializedType.Char;
    }
}
