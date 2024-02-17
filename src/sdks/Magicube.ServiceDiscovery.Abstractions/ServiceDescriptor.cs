using System;
using System.Collections.Generic;

namespace Magicube.ServiceDiscovery.Abstractions {
    public class ServiceDescriptor {
        public string                     Id         { get; set; }
        public string                     Name       { get; set; }
        public string                     Version    { get; set; }
        public AddressEndPoint            EndPoint   { get; set; }

        public IDictionary<string,string> Attributes { get; set; }

        public Uri                 ToUri(string scheme = "http", string path = "/") {
            var builder = new UriBuilder(scheme, EndPoint.Address, EndPoint.Port, path);
            return builder.Uri;
        }

        public override string ToString() {
            return $"{EndPoint.Address}:{EndPoint.Port}";
        }
    }
}