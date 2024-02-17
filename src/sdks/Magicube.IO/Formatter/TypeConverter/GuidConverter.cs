﻿using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class GuidConverter : BaseTypeConverter<Guid> {
        protected override void SerializeInternal(Guid obj, SerializationStream stream) {
            byte[] data = obj.ToByteArray();
            stream.WriteWithLengthPrefix(data);
        }

        protected override Guid DeserializeInternal(DeserializationStream stream, Type sourceType) {
            byte[] guidData = stream.ReadBytesWithSizePrefix();

            return new Guid(guidData);
        }

        public override SerializedType Type => SerializedType.Guid;
    }
}
