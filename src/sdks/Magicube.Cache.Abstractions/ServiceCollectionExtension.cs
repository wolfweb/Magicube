using Magicube.Core;
using Magicube.Core.Modular;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Cache.Abstractions {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddMemCache(this IServiceCollection services) {
            services
                .Configure<ModularOptions>(options => options.ModularShareAssembly.Add(typeof(ICacheProvider).Assembly))
                .TryAddSingleton<ICacheProvider, DefaultCacheProvider>(DefaultCacheProvider.Identity)
                .AddCacheProvider(new CacheProvider(DefaultCacheProvider.Identity, "进程缓存") {  PartialView = "DefaultConfig" });
            
            return services;
        }

        public static IServiceCollection AddCacheProvider(this IServiceCollection services, CacheProvider cacheProvider) {
            services.Configure<CacheOptions>(x => x.CacheProvider.Add(cacheProvider));
            return services;
        }
    }
}
