using Magicube.Core.Extensions;
using Magicube.WebServer.Internal;
using System;

namespace Magicube.WebServer.Serialization {
    public static class JsonHelpers {
        public static bool TryParseJson(this ISerializationService serializationService, string input, Type type, bool isDynamic, out object deserializedObject) {
            input = input.Trim();
            bool isJson = input.IsJson();

            if (isJson) {
                try {
                    if (isDynamic) {
                        deserializedObject = serializationService.Deserialize<DynamicDictionary>(input);
                        return true;
                    }

                    deserializedObject = serializationService.Deserialize(input, type);
                    return true;
                } catch (Exception) {
                    
                }
            }

            try {
                string serializedValue = serializationService.Serialize(input);
                deserializedObject = serializationService.Deserialize(serializedValue, type);
                return true;
            } catch (Exception) {
                deserializedObject = null;
                return false;
            }
        }
    }
}
