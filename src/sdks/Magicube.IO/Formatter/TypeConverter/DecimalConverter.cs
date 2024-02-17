using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class DecimalConverter : BaseTypeConverter<decimal> {
        protected override void SerializeInternal(decimal obj, SerializationStream stream) {
            int[] bits = decimal.GetBits(obj);
            foreach (int bit in bits) {
                byte[] data = BitConverter.GetBytes(bit);
                stream.Write(data);
            }
        }

        protected override decimal DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadDecimal();
        }

        public override SerializedType Type => SerializedType.Decimal;
    }
}
