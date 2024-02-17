using System;
using System.Text;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class StringConverter : BaseTypeConverter<string> {
        protected override void SerializeInternal(string obj, SerializationStream stream) {
            byte[] objBytes = Encoding.UTF8.GetBytes(obj);
            stream.WriteWithLengthPrefix(objBytes);
        }

        protected override string DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadUtf8WithSizePrefix();
        }

        public override SerializedType Type => SerializedType.String;
    }
}