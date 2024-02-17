using Magicube.Core.Modular;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Magicube.Core.IO;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Magicube.Core;
using Magicube.Core.Reflection;

namespace Magicube.Modular.Web.ModuleFolders {
	public class ThemeFolder : ModularFolder {
        public ThemeFolder(IOptions<ModularOptions> options, IWebFileProvider magicubeFileProvider, IHostEnvironment env)
            : base(options, magicubeFileProvider, env){
        }

        public override IEnumerable<ModularInfo> LoadModulars(Application application) {
            string[] folders = GetDirectories(Options.ThemeFolder);

            foreach (var folder in folders) {
                var loader = TryLoadModular(folder, application);
                var modular = new ModularInfo(folder, loader.LoadDefaultAssembly(), name => loader.LoadAssembly(name));
                foreach (var type in modular.Types.Where(x => typeof(ModularDescriptor).IsAssignableFrom(x) && !x.IsAbstract)) {
                    modular.Descriptor      = New<ModularDescriptor>.Creator(type);
                    modular.Descriptor.Type = ModularType.Theme;
                    yield return modular;
                }
            }
        }
    }
}
