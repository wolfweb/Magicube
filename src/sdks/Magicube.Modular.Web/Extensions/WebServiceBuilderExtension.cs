using Magicube.Core;
using Magicube.Core.Modular;
using Magicube.Modular.Web;
using Magicube.Modular.Web.Complier;
using Magicube.Modular.Web.Extensions;
using Magicube.Modular.Web.ModuleFolders;
using Magicube.Modular.Web.Routing;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddModular(this WebServiceBuilder builder) {
            builder.Services.AddSingleton<IModularManager, ModuleManager>()
                .AddSingleton<IModularFolder, ModularFolder>()
                .AddSingleton<IModularFolder, ThemeFolder>()
                .AddSingleton<IWebServiceBuilder, ModularWebServiceBuilder>()
                .AddSingleton<IApplicationModelProvider, ModularApplicationModelProvider>()
                .AddSingleton<IApplicationFeatureProvider<ViewsFeature>, ModularThemingViewsFeatureProvider>()
                .Replace<IViewCompilerProvider, ModularViewCompilerProvider>()
                .ConfigureOptions<ModularViewEngineOptionsSetup>()
                .AddTransient<IAreaControllerRouteMapper, DefaultAreaControllerRouteMapper>();

            return builder;
        }
    }

    public class ModularWebServiceBuilder : IWebServiceBuilder {
        public void Build(WebServiceBuilder builder, IServiceProvider serviceProvider) {
            var options = serviceProvider.GetService<IOptions<ModularOptions>>();
            if (options != null && options.Value.ModularShareAssembly != null) {
                var application = serviceProvider.GetService<IModularManager>().Initialize();

                foreach (var modular in application.Modulars) {
                    if (modular.Startup != null) {
                        modular.Startup.ConfigureServices(builder.Services);
                    }

                    AddModularLoader(builder.MvcBuilder, modular, modular.Assembly);
                }

                builder.Services.AddSingleton(application);
            }

            builder.MvcBuilder.PartManager.FeatureProviders.Add(new ModularThemingViewFeatureProvider(serviceProvider));
        }

        private IMvcBuilder AddModularLoader(IMvcBuilder mvcBuilder, ModularInfo modular, Assembly modularAssembly) {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(modularAssembly);
            foreach (var part in partFactory.GetApplicationParts(modularAssembly)) {
                if (part is CompiledRazorAssemblyPart) {
                    mvcBuilder.PartManager.ApplicationParts.Add(new ModularRazorAssemblyPart(modularAssembly, modular.Descriptor.Name));
                } else
                    mvcBuilder.PartManager.ApplicationParts.Add(part);
            }
            
            var relatedAssembliesAttrs = modularAssembly.GetCustomAttributes<RelatedAssemblyAttribute>();
            foreach (var attr in relatedAssembliesAttrs) {
                var relatedAssembly = modular.LoadRelatedAssemblyFunc(attr.AssemblyFileName);
                var part = new ModularRazorAssemblyPart(relatedAssembly, modular.Descriptor.Name);
                mvcBuilder.PartManager.ApplicationParts.Add(part);                
            }

            return mvcBuilder;
        }
    }
}
