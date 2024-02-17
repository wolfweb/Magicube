using Magicube.Eventbus;
using Magicube.Localization.Data;
using Magicube.Users.Events;
using Magicube.Users.Lang;
using Magicube.Users.Services;
using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Users {
    public class Startup : StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddTransient<UserService>();
            services.AddEvent<ISetupContext, OnSetupEvent>();
            
            services.AddStorageLocalizerProvider<LocalizerProvider>();
            
            services.AddScoped<INavigationProvider, NavigationProvider>();
            
            services.AddPermission(ModularInfo.Title, "Magicube.Users", GetType().Assembly);
        }
        
        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
