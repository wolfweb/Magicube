using Magicube.Core;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Magicube.Modular.Web.Complier {
    public class ModularThemingViewsFeatureProvider : IApplicationFeatureProvider<ViewsFeature> {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature) {
            feature.ViewDescriptors.Add(new CompiledViewDescriptor() {
                ExpirationTokens = Array.Empty<IChangeToken>(),
                RelativePath     = "/_ViewStart" + RazorViewEngine.ViewExtension,
                Item             = new RazorViewCompiledItem(typeof(ThemeViewStart), @"mvc.1.0.view", "/_ViewStart")
            });
        }
    }

    public class ModularThemingViewFeatureProvider : IApplicationFeatureProvider<ViewsFeature> {
        private readonly Application _application;
        
        private IEnumerable<IApplicationFeatureProvider<ViewsFeature>> _featureProviders;
        public ModularThemingViewFeatureProvider(IServiceProvider services) {
            _application        = services.GetService<Application>();
        }
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature) {
            EnsureScopedServices();

            foreach (var provider in _featureProviders) {
                provider.PopulateFeature(parts, feature);
            }
        }

        private void EnsureScopedServices() {            
            if ( _featureProviders == null) {
                lock (this) {
                    if (_featureProviders == null) {
                        _featureProviders = _application.ServiceProvider.GetServices<IApplicationFeatureProvider<ViewsFeature>>();
                    }
                }
            }
        }
    }
}
