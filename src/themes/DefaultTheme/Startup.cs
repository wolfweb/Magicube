using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DefaultTheme {
    public class Startup : StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.ConfigureOptions<ResourceManagementOptionsSetup>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
