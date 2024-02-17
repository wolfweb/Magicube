using System;
using System.Net;

namespace Magicube.ServiceDiscovery.Abstractions {
    public record AddressEndPoint (string Address, int Port){
        public Uri ToUri(string scheme = "http", string path = "/") {
            var builder = new UriBuilder(scheme, Address, Port, path);
            return builder.Uri;
        }

        public override string ToString() {
            return $"{Address}:{Port}";
        }

        public static implicit operator IPEndPoint(AddressEndPoint model) {
            return new(IPAddress.Parse(model.Address), model.Port);
        }
    }
}