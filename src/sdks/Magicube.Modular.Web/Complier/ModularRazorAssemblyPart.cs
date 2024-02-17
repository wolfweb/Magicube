using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Magicube.Modular.Web.Complier {
    public class ModularRazorAssemblyPart : ApplicationPart, IRazorCompiledItemProvider {
        public ModularRazorAssemblyPart(Assembly assembly, string areaName) {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            AreaName = areaName;
        }

        public          Assembly Assembly { get; }
        public          string   AreaName { get; }                        

        public override string   Name     => Assembly.GetName().Name;

        IEnumerable<RazorCompiledItem> IRazorCompiledItemProvider.CompiledItems {
            get {
                var loader = new ModularViewCompiledItemLoader(AreaName);
                return loader.LoadItems(Assembly);
            }
        }
    }
}
