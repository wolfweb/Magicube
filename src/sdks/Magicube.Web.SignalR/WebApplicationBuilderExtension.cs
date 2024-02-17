using Magicube.Web.SignalR;
using Microsoft.AspNetCore.Builder;

namespace Magicube.Web {
    public static class WebApplicationBuilderExtension {
        public static WebApplicationBuilder UseSignalR(this WebApplicationBuilder builder, string pattern = "signalr") {
            builder.EndpointRouteBuilder.MapHub<SignalHubCenter>(pattern);
            return builder;
        }
    }
}
