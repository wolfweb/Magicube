using System.Threading;

namespace Magicube.Core {
    public abstract class ShouldConfigOption {
        private volatile int _configured = 0;

        public virtual bool TryConfigure() {
            if (OnConfigure()) {
                Interlocked.CompareExchange(ref _configured, 1, 0);
            }
            return _configured == 1;
        }

        protected abstract bool OnConfigure();
    }
}
