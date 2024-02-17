using System;
using System.Text;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class UriConverter : BaseTypeConverter<Uri> {
        protected override void SerializeInternal(Uri obj, SerializationStream stream) {
            byte[] data = Encoding.UTF8.GetBytes(obj.AbsoluteUri);
            stream.WriteWithLengthPrefix(data);
        }

        protected override Uri DeserializeInternal(DeserializationStream stream, Type sourceType) {
            string absoluteUri = stream.ReadUtf8WithSizePrefix();
            return new Uri(absoluteUri);
        }

        public override SerializedType Type => SerializedType.Uri;
    }
}
