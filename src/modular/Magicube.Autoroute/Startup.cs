using System;
using Magicube.Autoroute.Abstractions;
using Magicube.Web;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Autoroute {
    public class Startup: StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddSingleton<AutorouteTransformer>();
            services.AddPermission(ModularInfo.Title, "Magicube.Autoroute", GetType().Assembly);
        }
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            routes.MapDynamicControllerRoute<AutorouteTransformer>("/{any}/{**slug}");
        }
    }
}
