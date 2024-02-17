using System.Collections.Generic;

namespace Magicube.Resource {
    public class ResourceManagementOptions {
        public bool                      UseCdn            { get; set; }
        public string                    CdnBaseUrl        { get; set; }
        public bool                      DebugMode         { get; set; }
        public string                    Culture           { get; set; }
        public bool                      AppendVersion     { get; set; } = true;
        public string                    ContentBasePath   { get; set; } = string.Empty;
        public HashSet<ResourceManifest> ResourceManifests { get; } = new HashSet<ResourceManifest>();
    }
}
