using System;
using System.Collections.Generic;
using System.Linq;
using Magicube.WebServer.RequestHandlers;
using Magicube.WebServer.Internal;

namespace Magicube.WebServer {
    public class MiniWebContext : IDisposable {
        public string CorrelationId;

        public IUserIdentity CurrentUser;

        public IList<Exception> Errors = new List<Exception>();

        public bool Handled;

        public object HostContext;

        public IDictionary<string, object> Items = new DynamicDictionary();

        public MiniWebConfiguration Configuration;

        public MiniWebRequest Request;

        public IRequestHandler RequestHandler;

        public DateTime RequestTimestamp;

        public MiniWebResponse Response;

        public bool EnableVerboseErrors = false;

        public string RootFolderPath;

        private bool _disposed;

        public MiniWebContext(MiniWebRequest request, MiniWebResponse response, MiniWebConfiguration configuration, DateTime requestTimestamp) {
            request.Context    = this;
            Request            = request;
            response.Context   = this;
            Response           = response;
            Configuration  = configuration;
            RequestTimestamp   = requestTimestamp;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed)
                return;

            if (disposing) {
                foreach (IDisposable disposableItem in Items.Values.OfType<IDisposable>())
                    disposableItem.Dispose();

                Items.Clear();
            }

            _disposed = true;
        }
    }

}
