using Magicube.Core;
using Magicube.Email.Configurations;
using Magicube.Net.Email;
using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Email {
    public class Startup : StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.Replace<IEmailConfigResolve, Services.EmailConfigResolve>();
            services.AddScoped<INavigationProvider, NavigationProvider>();
            services.AddConfigOptions<EmailConfigWithUIOption>();
            services.AddPermission(ModularInfo.Title, "Magicube.Email", GetType().Assembly);
        }
        
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
