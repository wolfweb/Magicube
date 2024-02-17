using System;
using System.Security.Cryptography;

namespace Magicube.Medias.Hls {
    internal static class UriExtension {
        internal static Uri Join(this Uri host, string uri) {
            if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri relativeUri)) {
                if (relativeUri.IsAbsoluteUri) {
                    return relativeUri;
                }
                else {
                    if (host != null)
                        return new Uri(host, relativeUri);
                }
            }
            throw new UriFormatException(uri);
        }
    }
}