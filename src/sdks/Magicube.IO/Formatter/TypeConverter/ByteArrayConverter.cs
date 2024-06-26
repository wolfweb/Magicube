﻿using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class ByteArrayConverter : BaseTypeConverter<byte[]> {
        protected override void SerializeInternal(byte[] obj, SerializationStream stream) {
            stream.WriteWithLengthPrefix(obj);
        }

        protected override byte[] DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadBytesWithSizePrefix();
        }

        public override SerializedType Type => SerializedType.ByteArray;
    }
}
