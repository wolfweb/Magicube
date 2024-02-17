using Magicube.Core;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace Magicube.Modular.Web.Complier {
    public class ModularViewCompilerProvider : IViewCompilerProvider {
        private readonly IViewCompiler _compiler;
        private readonly ApplicationPartManager _applicationPartManager;
        public ModularViewCompilerProvider(Application application, ApplicationPartManager applicationPartManager) {
            _applicationPartManager = applicationPartManager;

            var feature = new ViewsFeature();
            _applicationPartManager.PopulateFeature(feature);

            _compiler = new ModularViewCompiler(feature.ViewDescriptors, application);

        }
        public IViewCompiler GetCompiler() => _compiler;
    }

}
