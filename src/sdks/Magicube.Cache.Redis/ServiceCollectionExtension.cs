using Magicube.Cache.Abstractions;
using Magicube.Core;
using Magicube.Core.Modular;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Magicube.Cache.Redis {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, Action<RedisCacheBuilder> builder = null) {
            services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.Add(typeof(RedisCacheProvider).Assembly);
            });

            var redisBuilder = new RedisCacheBuilder(services);
            builder?.Invoke(redisBuilder);

            services.TryAddSingleton<ICacheProvider, RedisCacheProvider>(RedisCacheProvider.Identity);
            services.TryAddSingleton<IRedisResolve, DefaultRedisResolve>();
            services.AddCacheProvider(new CacheProvider(RedisCacheProvider.Identity, "Redis缓存"));

            return services;
        }
    }

    public class RedisCacheBuilder {
        private readonly IServiceCollection _services;

        public RedisCacheBuilder(IServiceCollection services) {
            _services = services;
        }

        public RedisCacheBuilder ConfigRedis(RedisCacheSetting config) {
            _services.Configure<RedisCacheSetting>(x => { 
                x.Host     = config.Host;
                x.Port     = config.Port;
                x.Password = config.Password;
            });
            return this;
        }
    }
}
