using System;
using Magicube.IO.Types;
using Magicube.IO.Streams;

namespace Magicube.IO.TypeConverter {
    internal class NullConverter : BaseTypeConverter<object> {

        protected override void SerializeInternal(object obj, SerializationStream stream) {
            stream.Write();
        }

        protected override object DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return null;
        }

        public override SerializedType Type => SerializedType.Null;
    }
}
