using Magicube.Core.Environment.Variable;
using System;

namespace Magicube.Web.Environment.Variable {
    public class HttpMethodVariableHandler : BaseVariableHandler {
        private readonly HttpServiceProvider _httpServiceProvider;

        public HttpMethodVariableHandler(HttpServiceProvider httpServiceProvider) {
            _httpServiceProvider = httpServiceProvider;
        }

        public override string Name => "{HTTP_METHOD}";

        public override string Description => "获取http请求方式";

        public override string ParseVariable(string template, string source) {
            var res = template;
            var match = Find(res);
            if (match.Success) {
                res = template.Replace(match.Value, _httpServiceProvider.Method);
            }

            return res;
        }
    }
}
