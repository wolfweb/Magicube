using Magicube.ModularCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Magicube.Modular.Web {
    public static class MvcModularExtensions {
        public static IMvcBuilder AddModularFromAssemblyFile(this IMvcBuilder mvcBuilder, string assemblyFile) {
            var modular = ModularLoader.CreateFromAssemblyFile(
                assemblyFile, 
                config => config.PreferSharedTypes = true);

            return mvcBuilder.AddModularLoader(modular);
        }
        public static IMvcBuilder AddModularLoader(this IMvcBuilder mvcBuilder, ModularLoader modularLoader) {
            var modularAssembly = modularLoader.LoadDefaultAssembly();

            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(modularAssembly);
            foreach (var part in partFactory.GetApplicationParts(modularAssembly)) {
                mvcBuilder.PartManager.ApplicationParts.Add(part);
            }

            var relatedAssembliesAttrs = modularAssembly.GetCustomAttributes<RelatedAssemblyAttribute>();
            foreach (var attr in relatedAssembliesAttrs) {
                var assembly = modularLoader.LoadAssembly(attr.AssemblyFileName);
                partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                foreach (var part in partFactory.GetApplicationParts(assembly)) {
                    mvcBuilder.PartManager.ApplicationParts.Add(part);
                }
            }
            return mvcBuilder;
        }
    }
}
