using System.Collections.Generic;

namespace Magicube.ServiceDiscovery.Abstractions {
    public abstract class ServiceDiscoveryOptions {
        public IEnumerable<AddressEndPoint> Addresses { get; set; }
    }
}