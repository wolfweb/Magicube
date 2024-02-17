using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Magicube.Cache.Redis {
    public interface IRedisResolve {
        public IConnectionMultiplexer GetConnectionMultiplexer();
    }
    public class DefaultRedisResolve : IRedisResolve {
        private readonly RedisCacheSetting _redisCacheSetting;
        public DefaultRedisResolve(IOptions<RedisCacheSetting> options) {
            _redisCacheSetting = options.Value;
        }
        public IConnectionMultiplexer GetConnectionMultiplexer() {
            if(_redisCacheSetting != null && _redisCacheSetting.TryInitialize()) {
                return _redisCacheSetting.ConnectionMultiplexer;
            }

            return null;
        }
    }
}
