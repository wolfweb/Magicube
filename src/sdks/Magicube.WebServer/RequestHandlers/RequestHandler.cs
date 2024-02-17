using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.WebServer.RequestHandlers {
    public abstract class RequestHandler : IRequestHandler {
        public RequestHandler(string urlPath, EventHandler eventHandler) {
            if (string.IsNullOrWhiteSpace(urlPath))
                throw new ArgumentNullException("urlPath");

            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler");

            UrlPath = urlPath;
            EventHandler = eventHandler;
        }

        public string UrlPath { get; set; }

        public EventHandler EventHandler { get; set; }

        public abstract Task<MiniWebContext> HandleRequestAsync(MiniWebContext context);

        public virtual async Task<MiniWebContext> ProcessRequestAsync(MiniWebContext context) {
            try {
                context.Configuration.GlobalEventHandler.InvokePreInvokeHandlers(context);

                if (EventHandler != null)
                    EventHandler.InvokePreInvokeHandlers(context);

                if (context.Handled)
                    return context;

                context = await HandleRequestAsync(context);
                return context;
            } catch (Exception e) {
                if (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                context.Errors.Add(e);
                context.ReturnHttp500InternalServerError();

                context.Configuration.GlobalEventHandler.InvokeUnhandledExceptionHandlers(e, context);

                if (EventHandler != null)
                    EventHandler.InvokeUnhandledExceptionHandlers(e, context);
            } finally {
                context.Configuration.GlobalEventHandler.InvokePostInvokeHandlers(context);

                if (EventHandler != null)
                    EventHandler.InvokePostInvokeHandlers(context);

                if (context.Response.ResponseStreamWriter == null)
                    context.WriteResponseObjectToResponseStream();
            }

            return context;
        }

        public override string ToString() {
            return string.Format("[RequestHandler: UrlPath={0}, Type={1}]", UrlPath, GetType().Name);
        }
    }
}
