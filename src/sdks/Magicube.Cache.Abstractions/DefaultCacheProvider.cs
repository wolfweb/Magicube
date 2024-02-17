using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Magicube.Cache.Abstractions {
    public class DefaultCacheProvider : ICacheProvider {
        public const string Identity = "InMemoryCache";
        private readonly IMemoryCache _memoryCache;
        public DefaultCacheProvider(IMemoryCache memoryCache) {
            _memoryCache = memoryCache;
        }

        public bool Exits<T>(string key) {
           return _memoryCache.TryGetValue(BuildKey<T>(key), out _);
        }

        public T GetOrAdd<T>(string key, Func<T> handler, DateTime expire) {
            return _memoryCache.GetOrCreate<T>(BuildKey<T>(key), entry => {
                entry.AbsoluteExpiration = expire;
                return handler();
            });
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> handler, DateTime expire) {
            return await _memoryCache.GetOrCreateAsync(BuildKey<T>(key), async entry => {
                entry.AbsoluteExpiration = expire;
                return await handler();
            });
        }

        public T GetOrAdd<T>(string key, Func<T> handler, TimeSpan expire) {
            return _memoryCache.GetOrCreate<T>(BuildKey<T>(key), entry => {
                entry.SlidingExpiration = expire;
                return handler();
            });
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> handler, TimeSpan expire) {
            return await _memoryCache.GetOrCreateAsync(BuildKey<T>(key), async entry => {
                entry.SlidingExpiration = expire;
                return await handler();
            });
        }

        public void Override<T>(string key, T t, DateTime expire) {
            _memoryCache.Set(BuildKey<T>(key), t, expire);
        }

        public void Override<T>(string key, T t, TimeSpan expire) {
            _memoryCache.Set(BuildKey<T>(key), t, expire);
        }

        public void Remove<T>(string key) {
            _memoryCache.Remove(BuildKey<T>(key));
        }

        public bool TryGet<T>(string key, out T v) {
            return _memoryCache.TryGetValue(BuildKey<T>(key), out v);
        }

        private string BuildKey<T>(object id) {
            return $"DefaultCache:{typeof(T).Name}:{id}";
        }
    }
}
