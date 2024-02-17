using Magicube.Core;
using Magicube.Core.IO;
using Magicube.Core.Modular;
using Magicube.Core.Reflection;
using Magicube.ModularCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Magicube.Modular.Web.ModuleFolders {
    public class ModularFolder : IModularFolder {
        protected readonly IHostEnvironment HostEnvironment;
        protected readonly IWebFileProvider WebFileProvider;
        protected readonly ModularOptions Options;

        public ModularFolder(IOptions<ModularOptions> options, IWebFileProvider webFileProvider, IHostEnvironment env) {
            Options         = options.Value;
            WebFileProvider = webFileProvider;
            HostEnvironment = env;
        }

        public virtual IEnumerable<ModularInfo> LoadModulars(Application application) {
            string[] folders = GetDirectories(Options.ModularFolder);
            
            foreach (var folder in folders) {
                var loader = TryLoadModular(folder, application);
                var modular = new ModularInfo(folder, loader.LoadDefaultAssembly(), name => loader.LoadAssembly(name));
                foreach (var type in modular.Types.Where(x=> typeof(ModularDescriptor).IsAssignableFrom(x) && !x.IsAbstract)) {
                    modular.Descriptor      = New<ModularDescriptor>.Creator(type);
                    modular.Descriptor.Type = ModularType.Modular;
                    yield return modular;
                }
            }
        }

        protected virtual ModularLoader TryLoadModular(string folder, Application application) {
            var modularFile = GetModularFile(folder, application);
            ModularLoader loader = null;
            if (!modularFile.IsNullOrEmpty() &&  File.Exists(modularFile)) {
                loader = ModularLoader.CreateFromAssemblyFile(modularFile, Options.ModularShareAssembly.ToArray());
            }
            return loader;
        }

        protected string GetModularFile(string folder, Application application) {
            var modularName = Path.GetFileName(folder);
            var modularFileName = $"{modularName}.dll";

            string[] files;
            if (application.IsDevelop) {
                files = Directory.GetFiles(Path.Combine(folder, @$"bin\debug\{DotnetCoreFolder(application)}"), modularFileName, new EnumerationOptions { RecurseSubdirectories = true });
            } else {
                files = Directory.GetFiles(folder, modularFileName, new EnumerationOptions { RecurseSubdirectories = true });
            }
            string modularFile = files.FirstOrDefault();
            return modularFile;
        }

        protected string[] GetDirectories(string folder) {
            var root = folder;
            if (!WebFileProvider.IsAbsolutePath(root))
                root = WebFileProvider.MapPath(root);

            return Directory.GetDirectories(root);
        }

        private string DotnetCoreFolder(Application application) {
            if (application.RunFramework.StartsWith(".NET 8")) return "net8.0";
            if (application.RunFramework.StartsWith(".NET 6")) return "net6.0";
            if (application.RunFramework.StartsWith(".NET Core 3.1")) return "netcoreapp3.1";

            return "net6.0";
        }
    }
}
