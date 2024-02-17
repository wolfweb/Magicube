using System.Collections.Generic;
using System.Collections.Specialized;

namespace Magicube.WebServer {
    public class MiniWebRequest {
        public string ClientIpAddress;

        public string HttpMethod;

        public IList<HttpFile> Files;

        public MiniWebContext Context;

        public NameValueCollection FormBodyParameters;

        public NameValueCollection HeaderParameters;

        public NameValueCollection QueryStringParameters;

        public Url Url;

        public MiniWebRequest(string httpMethod, Url url, RequestStream requestStream, NameValueCollection queryStringParameters, NameValueCollection formBodyParameters, NameValueCollection headerParameters, IList<HttpFile> files, string clientIpAddress) {
            HttpMethod            = httpMethod;
            Url                   = url;
            RequestBody           = requestStream;
            QueryStringParameters = queryStringParameters ?? new NameValueCollection();
            FormBodyParameters    = formBodyParameters ?? new NameValueCollection();
            HeaderParameters      = headerParameters ?? new NameValueCollection();
            Files                 = files ?? new List<HttpFile>();
            ClientIpAddress       = clientIpAddress;
        }

        public RequestStream RequestBody;
    }

}
