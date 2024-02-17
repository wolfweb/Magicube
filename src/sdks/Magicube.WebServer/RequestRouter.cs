using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magicube.WebServer.RequestHandlers;

namespace Magicube.WebServer {
    public static class RequestRouter {
        public static async Task<MiniWebContext> RouteRequestAsync(MiniWebContext context) {
            string path = context.Request.Url.Path.ToLower();
            var requestHandlerMatches = new List<IRequestHandler>();

            foreach (IRequestHandler handler in context.Configuration.RequestHandlers) {
                if (path.Equals(handler.UrlPath, StringComparison.InvariantCultureIgnoreCase)) {
                    context.RequestHandler = handler;
                    return await context.RequestHandler.ProcessRequestAsync(context);
                }

                if (path.StartsWith(handler.UrlPath, StringComparison.InvariantCultureIgnoreCase)) {
                    requestHandlerMatches.Add(handler);
                }
            }

            foreach (IRequestHandler handler in requestHandlerMatches) {
                context.RequestHandler = handler;
                return await context.RequestHandler.ProcessRequestAsync(context);
            }

            if (context.RequestHandler == null || context.Handled == false) {
                if (context.Configuration.UnhandledRequestHandler != null) {
                    context.RequestHandler = context.Configuration.UnhandledRequestHandler;
                    await context.Configuration.UnhandledRequestHandler.ProcessRequestAsync(context);
                }
            }

            return context;
        }
    }
}
