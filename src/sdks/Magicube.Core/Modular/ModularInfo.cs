using Magicube.Core.Reflection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.Core.Modular {
    public class ModularInfo {
        public ModularInfo(string folder, Assembly assembly, Func<string, Assembly> loadRelatedAssembly) {
            StaticFileRoot = Path.Combine(folder, "wwwroot");
            Assembly       = assembly;
            Types          = assembly.GetTypes();
            LoadRelatedAssemblyFunc = loadRelatedAssembly;
            Task.Run(() => LoadStartup());
        }

        public IModularStartup   Startup        { get; private set; }
        public ModularDescriptor Descriptor     { get; set; }
        public Type[]            Types          { get; }
        public Assembly          Assembly       { get; }
        public string            StaticFileRoot { get; }

        public Func<string, Assembly> LoadRelatedAssemblyFunc { get; }

        protected virtual void LoadStartup() {
            var startups = Types.Where(x => typeof(IModularStartup).IsAssignableFrom(x) && !x.IsAbstract);
            if (startups.Any()) Startup = New<IModularStartup>.Creator(startups.First());
        }
    }
}
