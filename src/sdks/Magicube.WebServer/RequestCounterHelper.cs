using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Magicube.WebServer {
    public static class RequestCounterHelper {
        private const string RequestCounterIsEnabled = "RequestCounterIsEnabled";

        public static int RequestCount = 0;

        public static void EnableRequestCounter(this MiniWebConfiguration configuration) {
            if (configuration.GlobalEventHandler.PreInvokeHandlers.Contains(RequestCounterPreInvokeHandler) == false)
                configuration.GlobalEventHandler.PreInvokeHandlers.Add(RequestCounterPreInvokeHandler);
        }

        public static void DisableRequestCounter(this MiniWebConfiguration configuration) {
            configuration.GlobalEventHandler.PreInvokeHandlers.Remove(RequestCounterPreInvokeHandler);
        }

        public static void RequestCounterPreInvokeHandler(MiniWebContext context) {
            Interlocked.Increment(ref RequestCount);
            context.Items.Add(RequestCounterIsEnabled, null);
        }
        public static bool IsRequestCounterEnabled(this MiniWebContext context) {
            return context.Items.ContainsKey(RequestCounterIsEnabled);
        }
    }
}
