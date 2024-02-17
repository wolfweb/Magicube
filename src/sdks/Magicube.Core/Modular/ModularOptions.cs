using System.Collections.Generic;
using System.Reflection;

namespace Magicube.Core.Modular {
    public class ModularOptions {
        public ModularOptions() {
            ModularShareAssembly = new List<Assembly>();
        }

        public string         ModularFolder        { get; set; }
                                                   
        public string         ThemeFolder          { get; set; }

        public List<Assembly> ModularShareAssembly { get; set; }
    }
}
