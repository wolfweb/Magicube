using Magicube.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Magicube.ModularCore {
    public class ModularConfig {
        private bool _isUnloadable;
        private bool _loadInMemory;

        public ModularConfig(string mainAssemblyPath) {
            if (mainAssemblyPath.IsNullOrEmpty()) throw new ArgumentException(nameof(mainAssemblyPath));

            if (!Path.IsPathRooted(mainAssemblyPath)) {
                throw new ArgumentException("Value must be an absolute file path", nameof(mainAssemblyPath));
            }

            MainAssemblyPath = mainAssemblyPath;
        }
        public AssemblyLoadContext       DefaultContext    { get; set; } = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;
        public bool                      EnableHotReload   { get; set; } = true;
        public string                    MainAssemblyPath  { get; }
        public bool                      PreferSharedTypes { get; set; }
        public ICollection<AssemblyName> PrivateAssemblies { get; protected set; } = new List<AssemblyName>();
        public ICollection<AssemblyName> SharedAssemblies  { get; protected set; } = new List<AssemblyName>();
        public TimeSpan                  ReloadDelay       { get; set; } = TimeSpan.FromMilliseconds(200);
        public bool                      IsLazyLoaded      { get; set; } = false;

        public bool                      IsUnloadable      {
            get => _isUnloadable || EnableHotReload;
            set => _isUnloadable = value;
        }
    }
}
