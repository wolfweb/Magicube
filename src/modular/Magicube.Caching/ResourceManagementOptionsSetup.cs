using Magicube.Resource;
using Microsoft.Extensions.Options;

namespace Magicube.Caching {
    class ResourceManagementOptionsSetup : IConfigureOptions<ResourceManagementOptions> {
        public void Configure(ResourceManagementOptions options) {
            var manifest = new ResourceManifest();

            manifest.DefineScript("signalrRedis")
                .SetUrl("~/magicube.caching/js/signalrRedisCommand.js")
                .SetVersion("1.0.0");

            options.ResourceManifests.Add(manifest);
        }
    }
}
