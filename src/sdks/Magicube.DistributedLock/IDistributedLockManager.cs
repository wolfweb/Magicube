using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Magicube.DistributedLock {
    public interface IDistributedLockManager {
        Task<ILocker> AcquireLockAsync(string key, TimeSpan expiration);
        Task<ILocker> AcquireLockWaitAsync(string key, TimeSpan expireation, int waitSecond = 5);
        Task UnlockAsync(string key);
    }

    public class DistributedLockManager : IDistributedLockManager {
        private readonly ILogger _logger;
        private readonly IDistributedLockProvider _distributedLockDefinition;

        public DistributedLockManager(ILogger<DistributedLockManager> logger, IDistributedLockProvider distributedLockDefinition) {
            _logger = logger;
            _distributedLockDefinition = distributedLockDefinition;
        }
        public async Task<ILocker> AcquireLockAsync(string key, TimeSpan expiration) {
            if (await _distributedLockDefinition.CreateLockerAsync(key, expiration)) {
                return new Locker(key, _distributedLockDefinition, expiration);
            }
            return null;
        }

        public async Task<ILocker> AcquireLockWaitAsync(string key, TimeSpan expireation, int waitSecond = 5) {
            ILocker locker;
            while ((locker = await AcquireLockAsync(key, expireation)) == null) {
                await Task.Delay(waitSecond);
            }
            return locker;
        }

        public async Task UnlockAsync(string key) {
            await _distributedLockDefinition.UnlockAsync(key);
        }
    }
}
