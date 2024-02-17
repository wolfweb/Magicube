using Microsoft.AspNetCore.Razor.Hosting;
using System;

namespace Magicube.Modular.Web.Complier {
    public class ModularViewCompiledItemLoader : RazorCompiledItemLoader {
        public string ModularName { get; }
        public ModularViewCompiledItemLoader(string modularName) {
            ModularName = modularName;
        }

        protected override RazorCompiledItem CreateItem(RazorCompiledItemAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException(nameof(attribute));
            }

            return new ModularViewCompiledItem(attribute, ModularName);
        }
    }
}
