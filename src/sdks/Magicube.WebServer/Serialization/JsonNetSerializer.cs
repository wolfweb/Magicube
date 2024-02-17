using Magicube.Core.Text;
using System;

namespace Magicube.WebServer.Serialization {
    public class JsonNetSerializer : ISerializationService {
        public JsonNetSerializer() {
            //JsonSerializerSettings = new JsonSerializerSettings {
            //    DateFormatHandling = DateFormatHandling.IsoDateFormat,
            //    Converters = new JsonConverter[] { new StringEnumConverter(), new DynamicDictionaryConverter() },
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    Formatting = Debugger.IsAttached ? Formatting.Indented : Formatting.None,
            //};
        }

        public string Serialize(object obj) {
            return Json.Stringify(obj);
        }

        public T Deserialize<T>(string input) {
            return Json.Parse<T>(input);
        }

        public object Deserialize(string input, Type type) {
            return Json.Parse(input, type);
        }
    }
}
