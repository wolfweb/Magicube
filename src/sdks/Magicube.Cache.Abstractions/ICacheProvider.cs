using System;
using System.Threading.Tasks;

namespace Magicube.Cache.Abstractions {
    public interface ICacheProvider {
        bool Exits<T>(string key);
        
        T GetOrAdd<T>(string key, Func<T> handler, DateTime expire);
        T GetOrAdd<T>(string key, Func<T> handler, TimeSpan expire);

        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> handler, DateTime expire);
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> handler, TimeSpan expire);

        bool TryGet<T>(string key, out T v);
        void Remove<T>(string key);

        void Override<T>(string key, T t, DateTime expire);
        void Override<T>(string key, T t, TimeSpan expire);
    }
}
