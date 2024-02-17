using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Magicube.Web {
    public class WebApplicationBuilder {
        public readonly IApplicationBuilder ApplicationBuilder;
        public readonly IEndpointRouteBuilder EndpointRouteBuilder;
        public WebApplicationBuilder(IApplicationBuilder app, IEndpointRouteBuilder endpointRouteBuilder) {
            ApplicationBuilder   = app;
            EndpointRouteBuilder = endpointRouteBuilder;
        }
    }
}
