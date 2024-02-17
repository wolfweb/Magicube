using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Magicube.WebServer.Internal;
using System.Collections.Specialized;

namespace Magicube.WebServer {
    public class MiniWebResponse {
        public string   Charset          = "utf-8";
                                         
        public Encoding ContentEncoding  = Encoding.UTF8;
                                         
        public string   ContentType      = string.Empty;

        public IList<MiniWebCookie>   Cookies          = new List<MiniWebCookie>();

        public NameValueCollection HeaderParameters = new NameValueCollection();

        public int HttpStatusCode = Constants.HttpStatusCode.Ok.ToInt();

        public MiniWebContext Context;

        public object ResponseObject;

        public Action<Stream> ResponseStreamWriter;

        public MiniWebResponse() {
            HeaderParameters.Add("X-Magicube-Version", Constants.Version);
            HeaderParameters.Add("X-Frame-Options", "SAMEORIGIN");
        }

        public void Redirect(string url) {
            Context.Handled = true;
            HttpStatusCode = Constants.HttpStatusCode.MovedPermanently.ToInt();
            HeaderParameters["Location"] = url;
        }

        public void TemporaryRedirect(string url) {
            HttpStatusCode = Constants.HttpStatusCode.FoundOrMovedTemporarily.ToInt();
            HeaderParameters["Location"] = url;
        }
    }

}
