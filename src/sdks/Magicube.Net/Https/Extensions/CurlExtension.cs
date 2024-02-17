using System;
using System.Net.Http;

namespace Magicube.Net {
    public static class CurlExtension {
        public static CurlContext Get(this Curl curl, Uri uri, Action<HttpRequestMessage> callback = null) {
            return curl.Get(uri.ToString(), callback);
        }
    }
}
