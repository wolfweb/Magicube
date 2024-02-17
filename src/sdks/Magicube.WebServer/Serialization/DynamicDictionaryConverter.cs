using Magicube.WebServer.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Magicube.WebServer.Serialization {
    public class DynamicDictionaryConverter : JsonConverter {
        public override bool CanWrite {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return ReadValue(reader);
        }

        private object ReadValue(JsonReader reader) {
            while (reader.TokenType == JsonToken.Comment) {
                if (!reader.Read())
                    throw CreateJsonSerializationException(reader, "Unexpected end when reading ExpandoObject.");
            }

            switch (reader.TokenType) {
                case JsonToken.StartObject:
                    return ReadObject(reader);
                case JsonToken.StartArray:
                    return ReadList(reader);
                default:
                    if (IsPrimitiveToken(reader.TokenType))
                        return reader.Value;
                    throw CreateJsonSerializationException(reader, string.Format(CultureInfo.InvariantCulture, "Unexpected token when converting ExpandoObject: {0}", reader.TokenType));
            }
        }

        private object ReadList(JsonReader reader) {
            IList<object> list = new List<object>();

            while (reader.Read()) {
                switch (reader.TokenType) {
                    case JsonToken.Comment:
                        break;
                    default:
                        list.Add(ReadValue(reader));
                        break;
                    case JsonToken.EndArray:
                        return list;
                }
            }

            throw CreateJsonSerializationException(reader, "Unexpected end when reading ExpandoObject.");
        }

        private object ReadObject(JsonReader reader) {
            IDictionary<string, object> expandoObject = new DynamicDictionary();

            while (reader.Read()) {
                switch (reader.TokenType) {
                    case JsonToken.PropertyName:
                        string propertyName = reader.Value.ToString();

                        if (!reader.Read())
                            throw CreateJsonSerializationException(reader, "Unexpected end when reading ExpandoObject.");

                        expandoObject[propertyName] = ReadValue(reader);
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.EndObject:
                        return expandoObject;
                }
            }

            throw CreateJsonSerializationException(reader, "Unexpected end when reading ExpandoObject.");
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(DynamicDictionary));
        }

        internal static bool IsPrimitiveToken(JsonToken token) {
            switch (token) {
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.Null:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return true;
                default:
                    return false;
            }
        }

        internal static JsonSerializationException CreateJsonSerializationException(JsonReader reader, string message) {
            return CreateJsonSerializationException(reader, message, null);
        }

        internal static JsonSerializationException CreateJsonSerializationException(JsonReader reader, string message, Exception ex) {
            return CreateJsonSerializationException(reader as IJsonLineInfo, reader.Path, message, ex);
        }

        internal static JsonSerializationException CreateJsonSerializationException(IJsonLineInfo lineInfo, string path, string message, Exception ex) {
            message = FormatMessage(lineInfo, path, message);
            return new JsonSerializationException(message, ex);
        }

        internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message) {
            if (!message.EndsWith(Environment.NewLine, StringComparison.Ordinal)) {
                message = message.Trim();

                if (!message.EndsWith("."))
                    message += ".";

                message += " ";
            }

            message += string.Format(CultureInfo.InvariantCulture, "Path '{0}'", path);

            if (lineInfo != null && lineInfo.HasLineInfo())
                message += string.Format(CultureInfo.InvariantCulture, ", line {0}, position {1}", lineInfo.LineNumber, lineInfo.LinePosition);

            message += ".";

            return message;
        }
    }
}
