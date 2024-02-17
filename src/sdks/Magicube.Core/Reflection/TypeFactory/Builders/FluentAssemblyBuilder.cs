using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentAssemblyBuilder : IAssemblyBuilder {
        private readonly Dictionary<string, FluentModuleBuilder> _modules = new Dictionary<string, FluentModuleBuilder>();

        public FluentAssemblyBuilder(AssemblyBuilder assemblyBuilder) {
            AssemblyBuilder = assemblyBuilder;
        }

        public AssemblyBuilder AssemblyBuilder { get; }

        public IModuleBuilder NewDynamicModule(string moduleName) {
            FluentModuleBuilder impl;
            if (_modules.TryGetValue(moduleName, out impl) == false) {
                var module = AssemblyBuilder.DefineDynamicModule(moduleName);
                impl = new FluentModuleBuilder(module);
                _modules.Add(moduleName, impl);
            }

            return impl;
        }
    }
}