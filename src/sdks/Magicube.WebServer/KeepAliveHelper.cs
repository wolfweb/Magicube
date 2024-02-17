using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.WebServer {
    public static class KeepAliveHelper {
        private const string KeepAliveIsDisabled = "KeepAliveIsDisabled";

        public static void EnableKeepAlive(this MiniWebConfiguration configuration) {
            configuration.GlobalEventHandler.EnableKeepAlive();
        }

        public static void DisableKeepAlive(this MiniWebConfiguration configuration) {
            configuration.GlobalEventHandler.DisableKeepAlive();
        }

        public static void EnableKeepAlive(this EventHandler eventHandler) {
            eventHandler.PreInvokeHandlers.Remove(DisableKeepAlivePreInvokeHandler);
        }

        public static void DisableKeepAlive(this EventHandler eventHandler) {
            if (eventHandler.PreInvokeHandlers.Contains(DisableKeepAlivePreInvokeHandler) == false)
                eventHandler.PreInvokeHandlers.Add(DisableKeepAlivePreInvokeHandler);
        }

        public static void DisableKeepAlivePreInvokeHandler(MiniWebContext context) {
            context.Items.Add(KeepAliveIsDisabled, null);
        }

        public static bool IsKeepAliveDisabled(this MiniWebContext context) {
            return context.Items.ContainsKey(KeepAliveIsDisabled);
        }
    }

}
