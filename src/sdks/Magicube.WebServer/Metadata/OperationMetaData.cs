using System.Collections.Generic;

namespace Magicube.WebServer.Metadata {
    public class OperationMetaData {
        public string Description;
        public IList<OperationParameter> InputParameters = new List<OperationParameter>();

        public string Name;
        public string ReturnParameterType;
        public string UrlPath;
        public Dictionary<string, object> CustomMetaData = new Dictionary<string, object>();
    }
}
