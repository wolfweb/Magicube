﻿using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class ULongConverter : BaseTypeConverter<ulong> {
        protected override void SerializeInternal(ulong obj, SerializationStream stream) {
            byte[] data = BitConverter.GetBytes(obj);
            stream.Write(data);
        }

        protected override ulong DeserializeInternal(DeserializationStream stream, Type sourceType) {
            return stream.ReadULong();
        }

        public override SerializedType Type => SerializedType.Ulong;
    }
}
