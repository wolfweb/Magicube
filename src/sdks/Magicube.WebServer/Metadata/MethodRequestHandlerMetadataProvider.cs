using Magicube.WebServer.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.WebServer.Metadata {
    public class MethodRequestHandlerMetadataProvider : IApiMetaDataProvider {
        public virtual string GetOperationName(MiniWebContext context, IRequestHandler requestHandler) {
            MethodRequestHandler handler = GetMethodRequestHandler(requestHandler);
            return handler.Method.Name;
        }
        public virtual string GetOperationDescription(MiniWebContext context, IRequestHandler requestHandler) {
            MethodRequestHandler handler = GetMethodRequestHandler(requestHandler);
            return handler.Description;
        }
        public virtual IList<MethodParameter> GetOperationParameters(MiniWebContext context, IRequestHandler requestHandler) {
            MethodRequestHandler handler = GetMethodRequestHandler(requestHandler);
            return handler.MethodParameters;
        }

        public virtual Type GetOperationReturnParameterType(MiniWebContext context, IRequestHandler requestHandler) {
            MethodRequestHandler handler = GetMethodRequestHandler(requestHandler);
            if (handler.Method.ReturnType == typeof(Task))
                return typeof(void);
            if (handler.Method.ReturnType.BaseType == typeof(Task))
                return handler.Method.ReturnType.GenericTypeArguments[0];
            return handler.Method.ReturnType;
        }
        public MethodRequestHandler GetMethodRequestHandler(IRequestHandler requestHandler) {
            if (requestHandler == null)
                throw new Exception("Context.RequestHandler is NULL");

            var handler = requestHandler as MethodRequestHandler;

            if (handler == null)
                throw new Exception("Expected a MethodRequestHandler but was a " + requestHandler.GetType());

            return handler;
        }
    }
}
