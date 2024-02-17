using System.Net;
using System;
using Magicube.ServiceDiscovery.Abstractions;

namespace Magicube.ServiceDiscovery.Consul {
    public class ConsulOptions : ServiceDiscoveryOptions {
        public string      Datacenter      { get; set; }
        public string      AclToken        { get; set; }
        public TimeSpan?   LongPollMaxWait { get; set; }
        public TimeSpan?   RetryDelay      { get; set; } = Defaults.ErrorRetryInterval;
    }

    public static class Defaults {
        public static TimeSpan ErrorRetryInterval => TimeSpan.FromSeconds(15);
        public static TimeSpan UpdateMaxInterval => TimeSpan.FromSeconds(15);
    }
}