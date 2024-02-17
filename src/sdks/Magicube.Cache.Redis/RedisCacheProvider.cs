using Magicube.Cache.Abstractions;
using Magicube.Core.Convertion;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Magicube.Cache.Redis {
    public class RedisCacheProvider : IRedisCacheProvider {
        public const string Identity = "InRedisCache";
        private readonly IDatabase _redis;
        public RedisCacheProvider(IRedisResolve redisResolve) {
            var conn = redisResolve.GetConnectionMultiplexer();
            if (conn == null) throw new CacheException("need initialize redis");

            _redis = conn.GetDatabase();
        }

        public bool Exits<T>(string key) {
            var _key = BuildKey<T>(key);
            return Exits(_key);
        }

		public bool Exits(string key) => _redis.KeyExists(key);

        public T GetOrAdd<T>(string key, Func<T> handler, DateTime expire) {
            T result = default(T);
            var _key = BuildKey<T>(key);

            var value = _redis.StringGet(_key);
            if (!value.HasValue) {
                result = handler();
                _redis.StringSet(_key, Bson.Serialize(result));
                _redis.KeyExpire(_key, expire);
            } else {
                result = Bson.Parse<T>(value);
            }
            return result;
        }

        public T GetOrAdd<T>(string key, Func<T> handler, TimeSpan expire) {
            T result = default(T);
            var _key = BuildKey<T>(key);

            var value = _redis.StringGet(_key);
            if (!value.HasValue) {
                result = handler();
                _redis.StringSet(_key, Bson.Serialize(result));
            } else {
                result = Bson.Parse<T>(value);
            }
            _redis.KeyExpire(_key, expire);
            return result;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> handler, DateTime expire) {
            T result = default(T);
            var _key = BuildKey<T>(key);

            var value = await _redis.StringGetAsync(_key);
            if (!value.HasValue) {
                result = await handler();
                await _redis.StringSetAsync(_key, Bson.Serialize(result));
                await _redis.KeyExpireAsync(_key, expire);
            } else {
                result = Bson.Parse<T>(value);
            }
            return result;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> handler, TimeSpan expire) {
            T result = default(T);
            var _key = BuildKey<T>(key);

            var value = await _redis.StringGetAsync(_key);
            if (!value.HasValue) {
                result = await handler();
                await _redis.StringSetAsync(_key, Bson.Serialize(result));
            } else {
                result = Bson.Parse<T>(value);
            }
            await _redis.KeyExpireAsync(_key, expire);
            return result;
        }

        public void Override<T>(string key, T t, DateTime expire) {
            var _key = BuildKey<T>(key);
            _redis.StringSet(_key, Bson.Serialize(t));
            _redis.KeyExpire(_key, expire);
        }

        public void Override<T>(string key, T t, TimeSpan expire) {
            var _key = BuildKey<T>(key);
            _redis.StringSet(_key, Bson.Serialize(t));
            _redis.KeyExpire(_key, expire);
        }

        public void Remove<T>(string key) {
            _redis.KeyDelete(BuildKey<T>(key));
        }

        public bool TryGet<T>(string key, out T v) {
            v = default(T);
            var _key = BuildKey<T>(key);
            if (_redis.KeyExists(_key)) {
                v = Bson.Parse<T>(_redis.StringGet(_key));
                return true;
            }
            return false;
        }

        private string BuildKey<T>(string key) {
            return $"RedisCacheProvider:{typeof(T).Name}:{key}";
        }
    }
}
