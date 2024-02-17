using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Forms.Services;
using Magicube.Eventbus;
using Magicube.Forms.Events;
using Magicube.Web.Environment.Builder;
using Microsoft.Extensions.DependencyInjection;
using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Caching;
using Magicube.Autoroute.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace Magicube.Forms {
    public class Startup : StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddTransient<DbTableService>()
                .AddEvent<DbTable, OnDbTableUpdatedEvent>()
                .AddScoped<INavigationProvider, NavigationProvider>()
                ;

            services.Configure<AutorouteOption>(options => {
                if (options.AutorouteValues.Count == 0) {
                    options.AutorouteValues = new RouteValueDictionary
                    {
                        {"Area", "Magicube.Forms"},
                        {"Controller", "FormItem"},
                        {"Action", "Display"}
                    };

                    options.ContentIdKey = "contentId";
                }
            });

            services.AddPermission(ModularInfo.Title, "Magicube.Forms", GetType().Assembly);
        }
    }
}
