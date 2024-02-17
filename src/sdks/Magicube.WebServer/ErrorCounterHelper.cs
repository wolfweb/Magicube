using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.WebServer {
    using System.Threading;

    public static class ErrorCounterHelper {
        private const string ErrorCounterIsEnabled = "ErrorCounterIsEnabled";

        public static int ErrorCount = 0;

        public static void EnableErrorCounter(this MiniWebConfiguration configuration) {
            if (configuration.GlobalEventHandler.UnhandledExceptionHandlers.Contains(ErrorCounterUnhandledExceptionHandler) == false)
                configuration.GlobalEventHandler.UnhandledExceptionHandlers.Add(ErrorCounterUnhandledExceptionHandler);
        }

        public static void DisableErrorCounter(this MiniWebConfiguration configuration) {
            configuration.GlobalEventHandler.UnhandledExceptionHandlers.Remove(ErrorCounterUnhandledExceptionHandler);
        }

        public static void ErrorCounterUnhandledExceptionHandler(Exception exception, MiniWebContext context) {
            Interlocked.Increment(ref ErrorCount);
            context.Items.Add(ErrorCounterIsEnabled, null);
        }

        public static bool IsErrorCounterEnabled(this MiniWebContext context) {
            return context.Items.ContainsKey(ErrorCounterIsEnabled);
        }
    }

}
