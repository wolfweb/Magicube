namespace Magicube.Core.Reflection {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if NETSTANDARD1_6
    using Microsoft.DotNet.InternalAbstractions;
    using Microsoft.Extensions.DependencyModel;
#endif

    internal static class AssemblyCache {
        public static Type GetType(string typeName, bool dynamicOnly = false) {
            foreach (var ass in AssemblyCache.GetAssemblies(dynamicOnly)) {
                Type type = ass.GetType(typeName);
                if (type != null) {
                    return type;
                }
            }

            return null;
        }

        public static IEnumerable<Assembly> GetAssemblies(bool dynamicOnly = false) {
#if NETSTANDARD1_6
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var assemblies =
                from lib in DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId)
                let ass = Assembly.Load(lib)
                select ass;
#else
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
#endif
            return FilterAssemblies(assemblies, dynamicOnly);
        }

        private static IEnumerable<Assembly> FilterAssemblies(IEnumerable<Assembly> list, bool dynamicOnly) {
            if (list == null) {
                yield break;
            }

            foreach (var assembly in list) {
                if (dynamicOnly == false ||
                    assembly.IsDynamic == true) {
                    yield return assembly;
                }
            }
        }
    }
}