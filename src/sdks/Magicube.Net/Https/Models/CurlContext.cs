using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Magicube.Net {
    public class CurlContext {
        public HttpRequestMessage  Request         { get; set; }
        public HttpResponseMessage Response        { get; set; }
        public object              Body            { get; set; }
        public ILogger<Curl>       Logger          { get; set; }
    }
}
