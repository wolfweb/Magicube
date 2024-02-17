using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Magicube.Core.Reflection.Builders;

namespace Magicube.Core.Reflection {
    internal class AssemblyBuilderCache {
        private Dictionary<string, FluentAssemblyBuilder> _cache;

        public AssemblyBuilderCache() {
            _cache = new Dictionary<string, FluentAssemblyBuilder>();
        }

        public IAssemblyBuilder GetOrCreateAssemblyBuilder(string assemblyName) {
            FluentAssemblyBuilder impl;
            if (_cache.TryGetValue(assemblyName, out impl) == false) {
                AssemblyName name = new AssemblyName(assemblyName);
                var builder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
                impl = new FluentAssemblyBuilder(builder);
                _cache.Add(assemblyName, impl);
            }

            return impl;
        }

        public bool RemoveAssemblyBuilder(string name) {
            return _cache.Remove(name);
        }

        public IEnumerable<Assembly> GetAssemblies() {
            return _cache.Select(a => a.Value.AssemblyBuilder);
        }
    }
}
