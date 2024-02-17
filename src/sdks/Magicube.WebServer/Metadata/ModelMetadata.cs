using System.Collections.Generic;

namespace Magicube.WebServer.Metadata {
    public class ModelMetadata {
        public string Description;

        public string Type;

        public IList<ModelProperty> Properties = new List<ModelProperty>();
    }
}
