using System;
using System.Collections.Generic;

namespace Magicube.WebServer {
    public class EventHandler {
        public delegate void ErrorHandler(Exception exception, MiniWebContext context);

        public delegate void PostInvokeHandler(MiniWebContext context);

        public delegate void PreInvokeHandler(MiniWebContext context);

        public IList<PostInvokeHandler> PostInvokeHandlers = new List<PostInvokeHandler>();

        public IList<PreInvokeHandler> PreInvokeHandlers = new List<PreInvokeHandler>();

        public IList<ErrorHandler> UnhandledExceptionHandlers = new List<ErrorHandler>();

        public void InvokeUnhandledExceptionHandlers(Exception exception, MiniWebContext context) {
            foreach (ErrorHandler unhandledExceptionHandler in UnhandledExceptionHandlers)
                unhandledExceptionHandler.Invoke(exception, context);
        }

        public void InvokePreInvokeHandlers(MiniWebContext context) {
            foreach (PreInvokeHandler preInvokeHandler in PreInvokeHandlers)
                preInvokeHandler.Invoke(context);
        }

        public void InvokePostInvokeHandlers(MiniWebContext context) {
            foreach (PostInvokeHandler postInvokeHandler in PostInvokeHandlers)
                postInvokeHandler.Invoke(context);
        }
    }

}
