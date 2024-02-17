using System;
using System.Collections.Generic;

namespace Magicube.WebServer {
    public class BackgroundTaskEventHandler {
        public delegate void ErrorHandler(Exception exception, BackgroundTaskContext backgroundTaskContext);
        public delegate void PostInvokeHandler(BackgroundTaskContext backgroundTaskContext);

        public delegate void PreInvokeHandler(BackgroundTaskContext backgroundTaskContext);
        public IList<PostInvokeHandler> PostInvokeHandlers = new List<PostInvokeHandler>();

        public IList<PreInvokeHandler> PreInvokeHandlers = new List<PreInvokeHandler>();

        public IList<ErrorHandler> UnhandledExceptionHandlers = new List<ErrorHandler>();

        public void InvokeUnhandledExceptionHandlers(Exception exception, BackgroundTaskContext backgroundTaskContext) {
            foreach (ErrorHandler unhandledExceptionHandler in UnhandledExceptionHandlers)
                unhandledExceptionHandler.Invoke(exception, backgroundTaskContext);
        }

        public void InvokePreInvokeHandlers(BackgroundTaskContext backgroundTaskContext) {
            foreach (PreInvokeHandler preInvokeHandler in PreInvokeHandlers)
                preInvokeHandler.Invoke(backgroundTaskContext);
        }

        public void InvokePostInvokeHandlers(BackgroundTaskContext backgroundTaskContext) {
            foreach (PostInvokeHandler postInvokeHandler in PostInvokeHandlers)
                postInvokeHandler.Invoke(backgroundTaskContext);
        }
    }

}
