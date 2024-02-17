using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.WebServer {
    public static class ElapsedMillisecondsHelper {
        private const string ElapsedMillisecondsResponseHeaderIsEnabled = "ElapsedMillisecondsResponseHeaderIsEnabled";

        public static void EnableElapsedMillisecondsResponseHeader(this MiniWebConfiguration configuration) {
            configuration.GlobalEventHandler.EnableElapsedMillisecondsResponseHeader();
        }

        public static void DisableElapsedMillisecondsResponseHeader(this MiniWebConfiguration configuration) {
            configuration.GlobalEventHandler.DisableElapsedMillisecondsResponseHeader();
        }

        public static void EnableElapsedMillisecondsResponseHeader(this EventHandler eventHandler) {
            if (eventHandler.PreInvokeHandlers.Contains(EnableElapsedMillisecondsResponseHeaderPostInvokeHandler) == false)
                eventHandler.PreInvokeHandlers.Add(EnableElapsedMillisecondsResponseHeaderPostInvokeHandler);
        }

        public static void DisableElapsedMillisecondsResponseHeader(this EventHandler eventHandler) {
            eventHandler.PreInvokeHandlers.Remove(EnableElapsedMillisecondsResponseHeaderPostInvokeHandler);
        }

        public static void EnableElapsedMillisecondsResponseHeaderPostInvokeHandler(MiniWebContext context) {
            context.Items.Add(ElapsedMillisecondsResponseHeaderIsEnabled, null);
        }

        public static bool IsElapsedMillisecondsResponseHeaderEnabled(this MiniWebContext context) {
            return context.Items.ContainsKey(ElapsedMillisecondsResponseHeaderIsEnabled);
        }
    }

}
