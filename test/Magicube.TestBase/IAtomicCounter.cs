using System.Threading;

namespace Magicube.TestBase {
    public interface IAtomicCounter<out T> {
        long AddAndGet(long amount);
        T Current { get; }
        T Next();
        void Reset();
    }

    public class AtomicCounter : IAtomicCounter<long> {
        private long _value;

        public long Current => Interlocked.Read(ref _value);

        public AtomicCounter(long value) {
            _value = value;
        }

        public AtomicCounter() {
            _value = -1;
        }

        public long AddAndGet(long amount) {
            var newValue = Interlocked.Add(ref _value, amount);
            return newValue;
        }

        public long Next() {
            return Interlocked.Increment(ref _value);
        }

        public void Reset() {
            Interlocked.Exchange(ref _value, 0);
        }
    }

}
