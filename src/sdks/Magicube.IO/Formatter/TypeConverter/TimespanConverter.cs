﻿using System;
using Magicube.IO.Streams;
using Magicube.IO.Types;

namespace Magicube.IO.TypeConverter {
    internal class TimespanConverter : BaseTypeConverter<TimeSpan> {
        protected override void SerializeInternal(TimeSpan obj, SerializationStream stream) {
            byte[] data = BitConverter.GetBytes(obj.Ticks);
            stream.Write(data);
        }

        protected override TimeSpan DeserializeInternal(DeserializationStream stream, Type sourceType) {
            long ticks = stream.ReadLong();
            return TimeSpan.FromTicks(ticks);
        }

        public override SerializedType Type => SerializedType.Timespan;
    }
}
