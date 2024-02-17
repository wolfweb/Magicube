using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Magicube.WebServer {
    public class HttpListenerConfiguration : IDisposable {
        public HttpListener HttpListener;

        public Action<Exception> UnhandledExceptionHandler;

        public bool IgnoreHttpListenerExceptions = true;

        public bool RewriteLocalhost;

        public string ApplicationPath = string.Empty;

        public HttpListenerConfiguration(System.Net.HttpListener httpListener) {
            HttpListener = httpListener;
        }

        public HttpListenerConfiguration(IList<Uri> uris, bool rewriteLocalhost = false) {
            HttpListener = new HttpListener();
            HttpListener.IgnoreWriteExceptions = true;
            RewriteLocalhost = rewriteLocalhost;
            AddPrefixes(uris);
        }

        public void AddPrefixes(IList<Uri> uris) {
            foreach (Uri uri in uris) {
                string prefix = uri.ToString();

                if (RewriteLocalhost && !uri.Host.Contains("."))
                    prefix = prefix.Replace("localhost", "+");

                HttpListener.Prefixes.Add(prefix);
            }
        }

        public string GetFirstUrlBeingListenedOn() {
            var firstUrlBeingListenedOn = HttpListener.Prefixes.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(firstUrlBeingListenedOn))
                return null;

            var uriBuilder = new UriBuilder(firstUrlBeingListenedOn);

            if (string.IsNullOrWhiteSpace(ApplicationPath) == false)
                uriBuilder.Path = ApplicationPath + "/";

            return uriBuilder.Uri.ToString();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            try {
                if (true) {

                    if (HttpListener == null) return;

                    HttpListener.Close();

                    HttpListener = null;
                }
            } catch (ObjectDisposedException) {
            }
        }
    }
}
