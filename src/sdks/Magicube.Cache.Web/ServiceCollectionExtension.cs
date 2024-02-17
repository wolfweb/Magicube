using Magicube.Cache.Abstractions;
using Magicube.Cache.Redis;
using Magicube.Core;
using Magicube.Core.Modular;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Magicube.Cache.Web {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddCache(this IServiceCollection services, Action<RedisCacheBuilder> builder = null) {
            services
                .Configure<ModularOptions>(options => {
                    options.ModularShareAssembly.Add(typeof(CacheProviderFactory).Assembly);
                })
                .AddMemCache()
                .AddRedisCache(builder)
                .TryAddSingleton<CacheProviderFactory>();
            return services;
        }
    }
}
