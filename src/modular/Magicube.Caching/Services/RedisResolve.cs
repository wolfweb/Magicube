using Magicube.Cache.Redis;
using Magicube.Caching.Configurations;
using StackExchange.Redis;

namespace Magicube.Caching.Services {
    public class RedisResolve : IRedisResolve {
        private readonly RedisConfigWithUIOption _redisConfigWithUIOption;
        public RedisResolve(RedisConfigWithUIOption redisConfigWithUIOption) {
            _redisConfigWithUIOption = redisConfigWithUIOption;
            if (!_redisConfigWithUIOption.TryConfigure()) {
                _redisConfigWithUIOption.DoRedirect();
            }
        }
        
        public IConnectionMultiplexer GetConnectionMultiplexer() => _redisConfigWithUIOption.ConnectionMultiplexer;
    }
}
