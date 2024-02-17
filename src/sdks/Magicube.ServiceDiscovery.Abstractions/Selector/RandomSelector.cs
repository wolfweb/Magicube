using System.Collections.Generic;
using System;
using System.Linq;

namespace Magicube.ServiceDiscovery.Abstractions {
    public class RandomSelector<T> : ILoadBalancer<T> {
        private readonly Func<int, int, int> _generate;
        private readonly IEnumerable<T> _services;
        private static readonly Random _random = new Random();
        public RandomSelector(IEnumerable<T> services) {
            _services = services;
            _generate = (min, max) => _random.Next(min, max);
        }

        public T Select() {
            var index = _generate(0, _services.Count());
            return _services.ElementAt(index);
        }
    }
}