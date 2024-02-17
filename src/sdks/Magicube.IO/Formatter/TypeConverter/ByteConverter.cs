﻿using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class ByteConverter : BaseTypeConverter<byte> {
        protected override void SerializeInternal(byte obj, SerializationStream stream) {
            stream.Write(obj);
        }

        protected override byte DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadByte();
        }

        public override SerializedType Type => SerializedType.Byte;
    }
}