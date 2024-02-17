using Magicube.Autoroute.Abstractions;
using Magicube.Core.Modular;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddAutoRoute(this WebServiceBuilder builder) {
            builder.Services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.Add(typeof(AutorouteTransformer).Assembly);
            });

            builder.Services.AddSingleton<IAutorouteEntriesProvider, AuthrouteEntriesProvider>();

            builder.Services.AddEntity<AutorouteEntry, AutorouteEntryMapping>();

            return builder;
        }
    }
}
