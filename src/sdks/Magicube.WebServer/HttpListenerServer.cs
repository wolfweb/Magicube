using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Magicube.WebServer.RequestHandlers;
using Magicube.WebServer.Internal;
using System.IO.Compression;

namespace Magicube.WebServer {
    public class HttpListenerServer : IDisposable {
        private bool _disposed;

        public HttpListenerConfiguration HttpListenerConfiguration;

        public MiniWebConfiguration Configuration;

        public HttpListenerServer(MiniWebConfiguration configuration, HttpListenerConfiguration httpListenerConfiguration) {
            Configuration = configuration;
            HttpListenerConfiguration = httpListenerConfiguration;
        }

        public static HttpListenerServer Start(MiniWebConfiguration configuration, HttpListenerConfiguration httpListenerConfiguration) {
            if (string.IsNullOrWhiteSpace(configuration.ApplicationRootFolderPath))
                configuration.ApplicationRootFolderPath = GetApplicationRootFolderPath();

            if (configuration.UnhandledRequestHandler == null)
                configuration.UnhandledRequestHandler = new FuncRequestHandler<MiniWebContext>("/UnhandledRequestHandler", configuration.DefaultEventHandler, context => context.ReturnHttp404NotFound());

            httpListenerConfiguration.HttpListener.Start();
            var server = new HttpListenerServer(configuration, httpListenerConfiguration);
            httpListenerConfiguration.HttpListener.BeginGetContext(server.BeginGetContextCallback, server);

            httpListenerConfiguration.UnhandledExceptionHandler = exception => {
                var errorMessage = new StringBuilder()
                    .AppendLine("MiniWeb Error:")
                    .AppendLine("*************").AppendLine();

                EventLogHelper.GenerateTextErrorMessage(exception, errorMessage);
                configuration.WriteErrorToEventLog(errorMessage.ToString());
            };

            BackgroundTaskRunner.Start(configuration);
            return server;
        }

        public static HttpListenerServer Start(MiniWebConfiguration configuration, params string[] urls) {
            var uriList = urls.Select(url => new Uri(url)).ToList();
            var httpListenerConfig = new HttpListenerConfiguration(uriList);
            return Start(configuration, httpListenerConfig);
        }

        public void BeginGetContextCallback(IAsyncResult asyncResult) {
            DateTime requestTimestamp = DateTime.UtcNow;

            try {
                if (HttpListenerConfiguration == null || HttpListenerConfiguration.HttpListener == null || HttpListenerConfiguration.HttpListener.IsListening == false)
                    return;
                HttpListenerContext httpListenerContext = HttpListenerConfiguration.HttpListener.EndGetContext(asyncResult);
                HttpListenerConfiguration.HttpListener.BeginGetContext(BeginGetContextCallback, this);
                HandleRequestAsync(httpListenerContext, this, requestTimestamp)
                    .ContinueWith(async task => await task);
            } catch (Exception e) {
                if (HttpListenerConfiguration == null || HttpListenerConfiguration.HttpListener == null || HttpListenerConfiguration.HttpListener.IsListening == false
                        || HttpListenerConfiguration.UnhandledExceptionHandler == null || (e.GetType() == typeof(HttpListenerException)) && HttpListenerConfiguration.IgnoreHttpListenerExceptions)
                    return;
                HttpListenerConfiguration.UnhandledExceptionHandler(e);
            }
        }

        public static async Task HandleRequestAsync(HttpListenerContext httpListenerContext, HttpListenerServer server, DateTime requestTimestamp) {
            MiniWebContext context = null;

            try {
                context = MapHttpListenerContextToContext(httpListenerContext, server, requestTimestamp);

                context = await RequestRouter.RouteRequestAsync(context);

                if (context.RequestHandler == null || context.Handled == false)
                    return;

                httpListenerContext.Response.ContentEncoding = context.Response.ContentEncoding;
                httpListenerContext.Response.ContentType = context.Response.ContentType;

                if (context.IsKeepAliveDisabled()) {
                    httpListenerContext.Response.KeepAlive = false;
                }

                foreach (string headerName in context.Response.HeaderParameters) {
                    if (!IgnoredHeaders.IsIgnored(headerName))
                        httpListenerContext.Response.Headers.Add(headerName, context.Response.HeaderParameters[headerName]);
                }

                if (context.IsElapsedMillisecondsResponseHeaderEnabled()) {
                    var elapsedTime = DateTime.UtcNow - context.RequestTimestamp;
                    httpListenerContext.Response.Headers.Add(Constants.ElapsedMillisecondsResponseHeaderName, elapsedTime.TotalMilliseconds.ToString());
                }

                foreach (MiniWebCookie cookie in context.Response.Cookies)
                    httpListenerContext.Response.Headers.Add("Set-Cookie", cookie.ToString());

                httpListenerContext.Response.StatusCode = context.Response.HttpStatusCode;

                if (context.Response.ResponseStreamWriter != null) {
                    if (IsGZipSupported(httpListenerContext)) {
                        httpListenerContext.Response.Headers.Add("Content-Encoding", "gzip");

                        using (var gZipStream = new GZipStream(httpListenerContext.Response.OutputStream, CompressionMode.Compress, true))
                            context.Response.ResponseStreamWriter(gZipStream);
                    } else
                        context.Response.ResponseStreamWriter(httpListenerContext.Response.OutputStream);
                }
            } catch (Exception exception) {
                try {
                    httpListenerContext.Response.OutputStream.Write(Constants.CustomErrorResponse.InternalServerError500); // Attempt to write an error message
                } catch (Exception) {
                    /* Gulp */
                }

                if (exception.GetType() == typeof(HttpListenerException) && server.HttpListenerConfiguration.IgnoreHttpListenerExceptions) {
                    return;
                }

                if (context != null) {
                    context.WriteContextDataToExceptionData(exception);
                }

                throw;
            } finally {
                httpListenerContext.Response.Close();
            }
        }

        public static bool IsGZipSupported(HttpListenerContext httpListenerContext) {
            string encoding = httpListenerContext.Request.Headers["Accept-Encoding"];

            if (!string.IsNullOrEmpty(encoding) && encoding.Contains("gzip"))
                return true;

            return false;
        }

        public static MiniWebContext MapHttpListenerContextToContext(HttpListenerContext httpListenerContext, HttpListenerServer server, DateTime requestTimestamp) {
            string httpMethod = httpListenerContext.Request.HttpMethod;
            string basePath = String.Empty;
            string path = "/" + httpListenerContext.Request.Url.AbsolutePath.TrimStart('/').ToLower();

            if (string.IsNullOrWhiteSpace(server.HttpListenerConfiguration.ApplicationPath) == false) {
                basePath = "/" + server.HttpListenerConfiguration.ApplicationPath.TrimStart('/').TrimEnd('/').ToLower();
                if (path.StartsWith(basePath)) path = path.Substring(basePath.Length);
            }

            path = string.IsNullOrWhiteSpace(path) ? "/" : path;

            var url = new Url {
                Scheme = httpListenerContext.Request.Url.Scheme,
                HostName = httpListenerContext.Request.Url.Host,
                Port = httpListenerContext.Request.Url.Port,
                BasePath = basePath,
                Path = path,
                Query = httpListenerContext.Request.Url.Query
            };

            RequestStream requestStream = RequestStream.FromStream(httpListenerContext.Request.InputStream, httpListenerContext.Request.Headers["Content-Length"]);
            var results = RequestBodyParser.ParseRequestBody(httpListenerContext.Request.Headers["Content-Type"], httpListenerContext.Request.ContentEncoding, requestStream, server.Configuration.RequestParameterLimit);
            var request = new MiniWebRequest(httpMethod, url, requestStream, httpListenerContext.Request.QueryString, results.FormBodyParameters, httpListenerContext.Request.Headers, results.Files, httpListenerContext.Request.RemoteEndPoint == null ? null : httpListenerContext.Request.RemoteEndPoint.Address.ToString());
            var context = new MiniWebContext(request, new MiniWebResponse(), server.Configuration, requestTimestamp) { HostContext = httpListenerContext, RootFolderPath = server.Configuration.ApplicationRootFolderPath };
            return context;
        }

        public static string GetApplicationRootFolderPath() {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static class IgnoredHeaders {
            private static readonly HashSet<string> KnownHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "content-length",
                "content-type",
                "transfer-encoding",
                "keep-alive"
            };

            public static bool IsIgnored(string headerName) {
                return KnownHeaders.Contains(headerName);
            }
        }

        public void Dispose() {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed == false) {
                if (disposing) {
                    if (HttpListenerConfiguration != null && HttpListenerConfiguration.HttpListener != null
                        && HttpListenerConfiguration.HttpListener.IsListening) {
                        try {
                            HttpListenerConfiguration.HttpListener.Close();
                        }
                        catch {
                        } finally {
                            HttpListenerConfiguration.HttpListener = null;
                            HttpListenerConfiguration = null;
                        }
                    }
                }

                _disposed = true;
            }
        }

    }

}
