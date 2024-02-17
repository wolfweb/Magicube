using System;
using Magicube.Web;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.MediaCenter {
    public class Startup: StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddPermission(ModularInfo.Title, "Magicube.MediaCenter", GetType().Assembly);
        }
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
