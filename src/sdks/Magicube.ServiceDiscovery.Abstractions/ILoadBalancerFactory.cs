using System.Collections.Generic;

namespace Magicube.ServiceDiscovery.Abstractions {
    public interface ILoadBalancerFactory {
        ILoadBalancer<T> Get<T>(IEnumerable<T> services, LoadBalancerMode options = LoadBalancerMode.Random);
    }

    public class LoadBalancerFactory : ILoadBalancerFactory {
        public ILoadBalancer<T>  Get<T>(IEnumerable<T> services, LoadBalancerMode options = LoadBalancerMode.Random) {
            switch (options) {
                case LoadBalancerMode.Random:
                    return new RandomSelector<T>(services);
                case LoadBalancerMode.RoundRobin:
                    return new RoundRobinSelector<T>(services);
                default:
                    return new RandomSelector<T>(services);
            }
        }
    }
}