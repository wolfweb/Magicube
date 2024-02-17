using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Cors {
    public class Startup : StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddPermission(ModularInfo.Title, "Magicube.Cors", GetType().Assembly);
            services.AddScoped<INavigationProvider, NavigationProvider>();
        }
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
