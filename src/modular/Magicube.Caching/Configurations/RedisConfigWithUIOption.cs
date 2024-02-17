using Magicube.Cache.Abstractions;
using Magicube.Cache.Redis;
using Magicube.Core;
using Magicube.Web.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;

namespace Magicube.Caching.Configurations {
    public class RedisConfigWithUIOption : ShouldConfigWithUIOption {        
        private IConnectionMultiplexer _connectionMultiplexer;

        public RedisConfigWithUIOption(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) {}

        public override RouteValueDictionary Routes => new RouteValueDictionary {
            ["area"]       = "Magicube.Caching",
            ["action"]     = "Index",
            ["controller"] = "Admin",
        };

        public IConnectionMultiplexer ConnectionMultiplexer => _connectionMultiplexer;

        protected override bool OnConfigure() {
            var configProvider = GetService<IMagicubeConfigProvider<CacheSetting>>();
            var cacheSetting = configProvider.GetSetting();
            if (cacheSetting == null) return false;
            
            if(cacheSetting.CacheProvider == RedisCacheProvider.Identity) {
                if (_connectionMultiplexer != null) return true;
                var redisSettings = cacheSetting as RedisCacheSetting;
                if (!redisSettings.IsValid) return false;

                if (!redisSettings.TryInitialize()) return false;
                _connectionMultiplexer = redisSettings.ConnectionMultiplexer;
            }

            return true;
        }
    }
}
