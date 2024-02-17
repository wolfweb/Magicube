using Magicube.ServiceDiscovery.Abstractions;
using System;

namespace Magicube.ServiceDiscovery.Zookeeper {
    public class ZookeeperOption : ServiceDiscoveryOptions {
        public TimeSpan SessionTimeout { get; set; }
    }
}