using Magicube.Core.IO;
using Magicube.Localization;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Magicube.Setup {
    public class SetupLocalizerProvider : FileLocalizerProvider {
        public SetupLocalizerProvider(IOptions<JsonLocalizationOptions> options, IWebFileProvider fileProvider) : base(options, fileProvider) {

        }
        public override IEnumerable<string> ResourcePaths() {
            var path = FileProvider.MapPath("~/Magicube.Setup/Assets/Langs");
            yield return path;
        }
    }
}
