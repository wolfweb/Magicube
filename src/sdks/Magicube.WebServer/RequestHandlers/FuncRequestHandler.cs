using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.WebServer.RequestHandlers {
    public class FuncRequestHandler<T> : RequestHandler {
        public FuncRequestHandler(string urlPath, EventHandler eventHandler, Func<MiniWebContext, T> func)
            : base(urlPath, eventHandler) {
            if (func == null)
                throw new ArgumentNullException("func");

            Func = func;
        }

        public Func<MiniWebContext, T> Func { get; private set; }

        public override async Task<MiniWebContext> HandleRequestAsync(MiniWebContext context) {
            context.Handled = true;
            var method = Func.GetMethodInfo();

            if (method.ReturnType == typeof(Task)) {
                await (dynamic)Func(context);
            } else if (method.ReturnType.BaseType == typeof(Task) && method.ReturnType.IsGenericType) {
                context.Response.ResponseObject = await (dynamic)Func(context);
            } else {
                context.Response.ResponseObject = Func(context);
            }

            if (context.Response.ResponseObject == null || context.Response.ResponseObject == context)
                return context;

            if (string.IsNullOrWhiteSpace(context.Response.ContentType))
                context.Response.ContentType = "application/json";

            return context;
        }
    }
}
