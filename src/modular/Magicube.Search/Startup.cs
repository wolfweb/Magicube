using System;
using Magicube.Core;
using Magicube.ElasticSearch;
using Magicube.Search.Configurations;
using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Search {
    public class Startup: StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddConfigOptions<ElasticSearchConfigWithUIOption>();
            services.Replace<IElasticSearchResolve, Services.ElasticSearchResolve>();
            services.AddScoped<INavigationProvider, NavigationProvider>();
            services.AddPermission(ModularInfo.Title, "Magicube.Search", GetType().Assembly);
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
