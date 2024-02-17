using Magicube.Core.Modular;
using Magicube.Web.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddSignalR(this WebServiceBuilder builder) {
            builder.Services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.Add(typeof(SignalHubContext).Assembly);
            });
            builder.Services.UseSignalR();
            return builder;
        }
    }
}
