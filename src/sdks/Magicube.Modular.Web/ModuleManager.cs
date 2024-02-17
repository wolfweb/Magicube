using Magicube.Core.Modular;
using System.Collections.Generic;
using Magicube.Core;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Magicube.Core.IO;
using System.IO;

namespace Magicube.Modular.Web {
    public class ModuleManager : IModularManager {
        private readonly ModularOptions Options;
        private readonly Application _application;
        private readonly IWebFileProvider _webFileProvider;
        private readonly IEnumerable<IModularFolder> _modularFolders;

        public ModuleManager(IOptions<ModularOptions> options, IWebFileProvider webFileProvider  ,Application application, IEnumerable<IModularFolder> moduleFolders) {
            Options          = options.Value;
            _modularFolders  = moduleFolders;
            _application     = application;
            _webFileProvider = webFileProvider;
        }

        public ModularInfo FindModular(Type type) {
            foreach(var modular in _application.Modulars) {
                if (modular.Types.Contains(type)) {
                    return modular;
                }
            }
            return null;
        }

        public IEnumerable<ModularInfo> FindModular(ModularType modularType) {
            return _application.Modulars.Where(x => x.Descriptor.Type == modularType);
        }

        public Application Initialize() {
            if (_application.Modulars != null) return _application;
            var modulars = new List<ModularInfo>();
            foreach (var folder in _modularFolders) {
                foreach (var it in folder.LoadModulars(_application)) {
                    modulars.Add(it);
                }
            }

            if (_webFileProvider.IsAbsolutePath(Options.ModularFolder)) {
                _application.ModularRootPath = Options.ModularFolder;
            } else {
                _application.ModularRootPath = Path.Combine(_application.Root, Options.ModularFolder);
            }

            if (_webFileProvider.IsAbsolutePath(Options.ThemeFolder)) {
                _application.ThemeRootPath = Options.ThemeFolder;
            } else {
                _application.ThemeRootPath = Path.Combine(_application.Root, Options.ThemeFolder);
            }

            _application.Modulars = modulars;
            return _application;
        }
    }
}
