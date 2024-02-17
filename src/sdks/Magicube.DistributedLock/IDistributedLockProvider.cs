using System;
using System.Threading.Tasks;

namespace Magicube.DistributedLock {
    public interface IDistributedLockProvider {
        Task<bool> UnlockAsync(string key);
        Task<bool> CreateLockerAsync(string key, TimeSpan expiration);
    }
}
