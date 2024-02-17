using Magicube.Core.Modular;
using Magicube.Resource;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddResourceManagement(this WebServiceBuilder builder) {
            builder.Services.TryAddScoped<IResourceManager, ResourceManager>();
            builder.Services.Configure<ModularOptions>(options=> {
                options.ModularShareAssembly.Add(typeof(IResourceManager).Assembly);
            });

            return builder;
        }
    }
}
