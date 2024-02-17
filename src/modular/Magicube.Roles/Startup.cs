using Magicube.Core;
using Magicube.Eventbus;
using Magicube.Roles.Events;
using Magicube.Roles.Services;
using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Roles {
    public class Startup : StartupBase {
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }

        public override void ConfigureServices(IServiceCollection services) {
            services.AddTransient<RoleService>();
            services.AddEvent<ISetupContext, OnSetupEvent>();
            services.AddScoped<INavigationProvider, NavigationProvider>();
            services.AddPermission(ModularInfo.Title, "Magicube.Roles", GetType().Assembly);
        }
    }
}
