using System;
using Magicube.Web;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.MessageService {
    public class Startup: StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddPermission(ModularInfo.Title, "Magicube.MessageService", GetType().Assembly);
        }
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
