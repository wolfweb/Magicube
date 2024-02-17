namespace Magicube.Cache.Abstractions {
    public abstract class CacheSetting {
        public abstract string CacheProvider { get; }
    }

    public class MemoryCacheSetting : CacheSetting {
        public override string CacheProvider { get; } = DefaultCacheProvider.Identity;
    }
}
