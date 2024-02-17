using Magicube.Modular.Web.LocationExpander;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;

namespace Magicube.Modular.Web.Extensions {
    public class ModularViewEngineOptionsSetup : IConfigureOptions<RazorViewEngineOptions> {
        public ModularViewEngineOptionsSetup() {
            
        }
        public void Configure(RazorViewEngineOptions options) {
            options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
        }
    }
}
