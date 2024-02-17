using Magicube.Localization;
using Magicube.Setup.Services;
using Magicube.Web.Environment.Builder;
using Magicube.Web.Sites;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Setup {
    public class Startup : StartupBase {
        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            var siteManager = builder.ApplicationServices.GetService<ISiteManager>();
            if (siteManager.GetSite().Status == SiteStatus.UnInitialized) {
                routes.MapAreaControllerRoute(
                    name    : "Setup",
                    pattern : "",
                    areaName: "Magicube.Setup",
                    defaults: new { controller = "Home", action = "Index" }
                );
            }
        }

        public override void ConfigureServices(IServiceCollection services) {
            services.AddFileLocalizerProvider<SetupLocalizerProvider>()
                .AddTransient<ISetupService, SetupService>()
                ;
        }
    }
}
