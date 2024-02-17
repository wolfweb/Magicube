using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentModuleBuilder : IModuleBuilder {
        private readonly ModuleBuilder _moduleBuilder;

        public FluentModuleBuilder(ModuleBuilder moduleBuilder) {
            _moduleBuilder = moduleBuilder;
        }

        public ITypeBuilder NewType(string typeName) {
            return new FluentTypeBuilder(_moduleBuilder, typeName);
        }

        public IMethodBuilder NewGlobalMethod(string methodName) {
            return new FluentMethodBuilder(
                methodName,
                _moduleBuilder.DefineGlobalMethod,
                () => {
                    CreateGlobalFunctions();
                    return GetMethod(methodName);
                })
                .CallingConvention(CallingConventions.Standard);
        }

        public IModuleBuilder CreateGlobalFunctions() {
            _moduleBuilder.CreateGlobalFunctions();
            return this;
        }

        public MethodInfo GetMethod(string methodName) {
            return _moduleBuilder.GetMethod(methodName);
        }
    }
}