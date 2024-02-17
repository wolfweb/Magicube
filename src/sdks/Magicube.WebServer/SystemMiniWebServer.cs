using System;
using System.Reflection;
using Magicube.WebServer.Internal;

namespace Magicube.WebServer {
    public class SystemMiniWebServer {
        public MiniWebConfiguration Configuration;
        public static void Start(dynamic httpApplication, MiniWebConfiguration configuration) {
            configuration.Host = httpApplication;
            configuration.ApplicationRootFolderPath = httpApplication.Server.MapPath("~/");

            EventInfo eventInfo = httpApplication.GetType().GetEvent("BeginRequest");
            MethodInfo methodInfo = typeof(SystemMiniWebServer).GetMethod("HttpApplicationOnBeginRequest", BindingFlags.Public | BindingFlags.Instance);
            var server = new SystemMiniWebServer { Configuration = configuration };
            Delegate eventHandlerDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, server, methodInfo);
            eventInfo.AddEventHandler(httpApplication, eventHandlerDelegate);
            BackgroundTaskRunner.Start(configuration);
        }

        public void HttpApplicationOnBeginRequest(dynamic httpApplication, EventArgs eventArgs) {
            HandleRequest(httpApplication.Context, Configuration, DateTime.UtcNow);
        }

        public static void HandleRequest(dynamic httpContext, MiniWebConfiguration configuration, DateTime requestTimestamp) {
            MiniWebContext Context = MapHttpContextBaseToContext(httpContext, configuration, requestTimestamp);
            Context = RequestRouter.RouteRequestAsync(Context).Result;

            if (Context.RequestHandler == null || Context.Handled == false)
                return;

            if (Context.Response.ResponseStreamWriter != null)
                Context.Response.ResponseStreamWriter(httpContext.Response.OutputStream);

            httpContext.Response.Charset = Context.Response.Charset;
            httpContext.Response.ContentEncoding = Context.Response.ContentEncoding;
            httpContext.Response.ContentType = Context.Response.ContentType;

            foreach (string headerName in Context.Response.HeaderParameters)
                httpContext.Response.Headers.Add(headerName, Context.Response.HeaderParameters[headerName]);

            if (Context.IsElapsedMillisecondsResponseHeaderEnabled()) {
                var elapsedTime = DateTime.UtcNow - Context.RequestTimestamp;
                httpContext.Response.Headers.Add(Constants.ElapsedMillisecondsResponseHeaderName, elapsedTime.TotalMilliseconds.ToString());
            }

            foreach (dynamic cookie in Context.Response.Cookies)
                httpContext.Response.Headers.Add("Set-Cookie", cookie.ToString());

            httpContext.Response.StatusCode = Context.Response.HttpStatusCode;
            httpContext.Response.End();
        }

        public static MiniWebContext MapHttpContextBaseToContext(dynamic httpContext, MiniWebConfiguration configuration, DateTime requestTimestamp) {
            dynamic httpMethod = httpContext.Request.HttpMethod;

            var basePath = httpContext.Request.ApplicationPath.TrimEnd('/');
            var path = httpContext.Request.Url.AbsolutePath.Substring(basePath.Length);
            path = string.IsNullOrWhiteSpace(path) ? "/" : path;

            var url = new Url {
                Scheme = httpContext.Request.Url.Scheme,
                HostName = httpContext.Request.Url.Host,
                Port = httpContext.Request.Url.Port,
                BasePath = basePath,
                Path = path,
                Query = httpContext.Request.Url.Query
            };

            RequestStream requestStream = RequestStream.FromStream(httpContext.Request.InputStream, httpContext.Request.Headers["Content-Length"]);
            var results = RequestBodyParser.ParseRequestBody(httpContext.Request.Headers["Content-Type"], httpContext.Request.ContentEncoding, requestStream, configuration.RequestParameterLimit);
            var request = new MiniWebRequest(httpMethod, url, requestStream, httpContext.Request.QueryString, httpContext.Request.Form, httpContext.Request.Headers, results.Files, httpContext.Request.UserHostAddress);
            var context = new MiniWebContext(request, new MiniWebResponse(), configuration, requestTimestamp) { HostContext = httpContext, RootFolderPath = configuration.ApplicationRootFolderPath };
            return context;
        }
    }
}
