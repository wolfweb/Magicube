using System;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.DistributedLock {
    public interface ILocker : IDisposable { }

    public class Locker : ILocker, IAsyncDisposable {
        private readonly IDistributedLockProvider _distributedLock;
        private readonly CancellationTokenSource _cts;
        private readonly string _key;

        private volatile int _released;
        private bool _disposed;

        public Locker(string key, IDistributedLockProvider distributedLock, TimeSpan expiration) {
            _key = key;
            _distributedLock = distributedLock;
            if (expiration != TimeSpan.MaxValue) {
                _cts = new CancellationTokenSource(expiration);
                _cts.Token.Register(Release, key);
            }
        }

        private void Release(object state) {
            if (Interlocked.Exchange(ref _released, 1) == 0) {
                _distributedLock.UnlockAsync(state.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public ValueTask DisposeAsync() {
            Dispose();
            return default;
        }

        public void Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;

            _cts?.Dispose();

            Release(_key);
        }
    }
}
