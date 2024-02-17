using Magicube.Core;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq.Expressions;

namespace Magicube.Data.Abstractions.ValueConversion {
    public class JsonToStringConverter<T> : ValueConverter<T, string> {
        public JsonToStringConverter(): this(null) { }

        public JsonToStringConverter(ConverterMappingHints mappingHints = null) 
            : base(ConvertToString(), ConvertToObject(), mappingHints) {
        }

        private static Expression<Func<T, string>> ConvertToString() {
            return (T v) => Json.Stringify(v, null);
        }

        private static Expression<Func<string, T>> ConvertToObject() {
            return (string v) => v.JsonToObject<T>();
        }
    }
}
