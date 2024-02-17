using System;
using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Schedule {
    public class Startup: StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddScoped<INavigationProvider, NavigationProvider>();
            services.AddPermission(ModularInfo.Title, "Magicube.Schedule", GetType().Assembly);
        }
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
