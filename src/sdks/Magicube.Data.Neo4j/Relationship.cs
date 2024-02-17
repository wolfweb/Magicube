using Newtonsoft.Json;

namespace Magicube.Data.Neo4j {
    public class Relationship {
        [JsonIgnore]
        public string Name { get; set; }
    }
}
