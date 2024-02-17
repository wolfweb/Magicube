using System;

namespace Magicube.ServiceDiscovery.Abstractions {
    public static class UriExtensions {
        public static string GetPath(this Uri uri) {
            return uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        }
    }
}