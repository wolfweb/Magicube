using System;
using System.Linq;
using System.Reflection;

namespace Magicube.Core.Reflection {
    public class TypeFactory {
        private static Lazy<TypeFactory> _instance = new Lazy<TypeFactory>(() => new TypeFactory("Default", "Default"), true);

        private AssemblyBuilderCache _assemblyCache;

        private IAssemblyBuilder _assemblyBuilder;

        private IModuleBuilder _moduleBuilder;

        public TypeFactory(string assemblyName, string moduleName) {
            Utility.ThrowIfArgumentNullEmptyOrWhitespace(assemblyName, nameof(assemblyName));
            Utility.ThrowIfArgumentNullEmptyOrWhitespace(moduleName, nameof(moduleName));

            _assemblyCache   = new AssemblyBuilderCache();
            _assemblyBuilder = _assemblyCache.GetOrCreateAssemblyBuilder(assemblyName);
            _moduleBuilder   = _assemblyBuilder.NewDynamicModule(moduleName);
        }

        public static TypeFactory Default {
            get {
                return _instance.Value;
            }
        }

        public ITypeBuilder NewType() {
            return NewType(Guid.NewGuid().ToString());
        }

        public ITypeBuilder NewType(string typeName) {
            return _moduleBuilder.NewType(typeName);
        }

        public Type NewDelegateType<TReturn>(string typeName, Type[] parameterTypes) {
            return NewDelegateType(typeName, parameterTypes, typeof(TReturn));
        }

        public Type NewDelegateType(string typeName, Type[] parameterTypes, Type returnType) {
            var typeBuilder = _moduleBuilder
                .NewType(typeName)
                .Attributes(TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass)
                .InheritsFrom<MulticastDelegate>();

            typeBuilder.NewConstructor()
                .RTSpecialName()
                .HideBySig()
                .Public()
                .CallingConvention(CallingConventions.Standard)
                .Param<object>("object")
                .Param<IntPtr>("method")
                .SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            typeBuilder.NewMethod("Invoke")
                .Public()
                .HideBySig()
                .NewSlot()
                .Virtual()
                .Returns(returnType)
                .Params(parameterTypes)
                .SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            return typeBuilder.CreateType();
        }

        public IMethodBuilder NewGlobalMethod(string methodName) {
            return _moduleBuilder.NewGlobalMethod(methodName);
        }

        public void CreateGlobalFunctions() {
            _moduleBuilder.CreateGlobalFunctions();
        }

        public MethodInfo GetMethod(string methodName) {
            return _moduleBuilder.GetMethod(methodName);
        }

        public Type GetType(string typeName, bool dynamicOnly = false) {
            var list = _assemblyCache.GetAssemblies()
                .Union(AssemblyCache.GetAssemblies(dynamicOnly));

            foreach (var ass in list) {
                Type type = ass.GetType(typeName);
                if (type != null) {
                    return type;
                }
            }

            return null;
        }
    }
}