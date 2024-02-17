using Magicube.Web;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Executeflow {
    public class Startup : StartupBase {
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
        public override void ConfigureServices(IServiceCollection services) {
            services.AddPermission(ModularInfo.Title, "Magicube.Executeflow", GetType().Assembly);
        }
    }
}
