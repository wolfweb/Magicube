using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Dashboard {
    public class Startup : IStartup {
        public int Order => 1;

        public void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {            
            routes.MapAreaControllerRoute(
                name    : "Dashboard",
                pattern : "Admin",
                areaName: "Magicube.Dashboard",
                defaults: new { controller = "Admin", action = "Index" }
                );
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddPermission(ModularInfo.Title, "Magicube.Dashboard", GetType().Assembly)
                .AddScoped<INavigationProvider, NavigationProvider>()
                ;
        }
    }
}
