using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Magicube.DistributedLock.SingleRedis {
    public class SingleRedisDistributedLockProvider : IDistributedLockProvider {
        private readonly IDatabase _redis;
        public SingleRedisDistributedLockProvider(IConnectionMultiplexer connectionMultiplexer) {
            _redis  = connectionMultiplexer.GetDatabase();
        }

        public async Task<bool> CreateLockerAsync(string key, TimeSpan expiration) {
            return await _redis.LockTakeAsync(key, true, expiration);
        }

        public async Task<bool> UnlockAsync(string key) {
            return await _redis.LockReleaseAsync(key, true);
        }
    }
}
