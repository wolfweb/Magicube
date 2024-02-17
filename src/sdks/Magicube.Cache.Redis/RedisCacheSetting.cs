using Magicube.Cache.Abstractions;
using Magicube.Core;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;

namespace Magicube.Cache.Redis {
    public class RedisCacheSetting : CacheSetting {
        public IConnectionMultiplexer ConnectionMultiplexer { get; private set; }

        public int    Port          { get; set; } = 6379;
        
        [Required]
        public string Host          { get; set; } = "localhost";

        public string Password      { get; set; }

        public override string CacheProvider { get; } = RedisCacheProvider.Identity;

        public bool TryInitialize() {
            if (ConnectionMultiplexer != null) return true;

            if (IsValid) {
                var addr = $"{Host}:{Port}";
                if (!string.IsNullOrEmpty(Password)) {
                    addr = $"{addr},password={Password}";
                }
                try {
                    ConnectionMultiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect(addr);
                    return true;
                } catch {
                    return false;
                }
            }
            return false;
        }

        public bool   IsValid => !Host.IsNullOrEmpty() && Port > 0;
    }
}
