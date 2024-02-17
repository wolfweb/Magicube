using System;
using System.Linq;
using Magicube.Cache.Abstractions;
using Magicube.Cache.Redis;
using Magicube.Caching.Configurations;
using Magicube.Caching.Events;
using Magicube.Caching.Services;
using Magicube.Core;
using Magicube.Eventbus;
using Magicube.Web;
using Magicube.Web.Navigation;
using Magicube.Web.Environment.Builder;
using Magicube.Web.SignalR;
using Magicube.Web.Sites;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Caching {
    public class Startup: StartupBase {
        public override void ConfigureServices(IServiceCollection services) {
            services.AddPolymorphicModelBinder(options => {
                options.Add<CacheSetting>(builder => {
                    builder.AddFromType<MemoryCacheSetting>();
                    builder.AddFromType<RedisCacheSetting>();
                });
            });

            services.AddTransient(typeof(IMagicubeConfigProvider<CacheSetting>), sp => new CacheConfigService(sp.GetService<ISiteManager>(), sp.GetService<IMapperProvider>()))
                .Replace<IRedisResolve, RedisResolve>()
                .AddConfigOptions<RedisConfigWithUIOption>()
                .AddScoped<INavigationProvider, NavigationProvider>()
                ;

            services.Configure<CacheOptions>(x => {
                var provider = x.CacheProvider.FirstOrDefault(x => x.Name == RedisCacheProvider.Identity);
                if (provider != null) {
                    provider.PartialView = "RedisConfig";
                }
            });

            services.AddEvent<SignalHubContext, PingRedisCommand>();

            services.ConfigureOptions<ResourceManagementOptionsSetup>();
            services.AddPermission(ModularInfo.Title, "Magicube.Caching", GetType().Assembly);
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            
        }
    }
}
