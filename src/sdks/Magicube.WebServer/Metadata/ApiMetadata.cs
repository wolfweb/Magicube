using System;
using System.Collections.Generic;

namespace Magicube.WebServer.Metadata {
    public class ApiMetadata {
        public string Version = "0.3.0";

        public string ApplicationName = AppDomain.CurrentDomain.FriendlyName;

        public IList<ModelMetadata> Models = new List<ModelMetadata>();

        public IList<OperationMetaData> Operations = new List<OperationMetaData>();
    }
}
