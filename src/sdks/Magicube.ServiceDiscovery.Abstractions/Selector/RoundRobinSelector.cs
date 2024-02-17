using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Magicube.ServiceDiscovery.Abstractions {
    public class RoundRobinSelector<T> : ILoadBalancer<T> {
        private int _last;
        private readonly IEnumerable<T> _services;
        public RoundRobinSelector(IEnumerable<T> services) {
            _services = services;
        }

        public T Select() {
            Interlocked.Increment(ref _last);

            if (_last >= _services.Count()) {
                Interlocked.Exchange(ref _last, 0);
            }

            return _services.ElementAt(_last);
        }
    }
}