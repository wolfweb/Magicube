using Magicube.WebServer.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magicube.WebServer.Metadata {
    public interface IApiMetaDataProvider {
        string GetOperationName(MiniWebContext context, IRequestHandler requestHandler);

        string GetOperationDescription(MiniWebContext context, IRequestHandler requestHandler);

        IList<MethodParameter> GetOperationParameters(MiniWebContext context, IRequestHandler requestHandler);

        Type GetOperationReturnParameterType(MiniWebContext context, IRequestHandler requestHandler);
    }
}
