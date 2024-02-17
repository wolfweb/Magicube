using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.WebServer {
    public static class CorrelationIdHelper {
        public static void EnableCorrelationId(this MiniWebConfiguration configuration) {
            configuration.GlobalEventHandler.EnableCorrelationId();
        }

        public static void DisableCorrelationId(this MiniWebConfiguration configuration) {
            configuration.GlobalEventHandler.DisableCorrelationId();
        }

        public static void EnableCorrelationId(this EventHandler eventHandler) {
            if (eventHandler.PreInvokeHandlers.Contains(EnableCorrelationIdPreInvokeHandler) == false)
                eventHandler.PreInvokeHandlers.Add(EnableCorrelationIdPreInvokeHandler);
        }

        public static void DisableCorrelationId(this EventHandler eventHandler) {
            eventHandler.PreInvokeHandlers.Remove(EnableCorrelationIdPreInvokeHandler);
        }

        public static void EnableCorrelationIdPreInvokeHandler(MiniWebContext context) {
            var correlationId = context.GetRequestParameterValue(Constants.CorrelationIdRequestParameterName);

            if (string.IsNullOrWhiteSpace(correlationId))
                correlationId = Guid.NewGuid().ToString();

            context.CorrelationId = correlationId;
            context.Response.HeaderParameters.Add(Constants.CorrelationIdRequestParameterName, correlationId);
            //CallContext.LogicalSetData(Constants.CorrelationIdRequestParameterName, correlationId);
        }
    }
}
