using Magicube.Core.Environment.Variable;
using System;

namespace Magicube.Web.Environment.Variable {
    public class HttpContentTypeVariableHandler : BaseVariableHandler {
        private readonly HttpServiceProvider _httpServiceProvider;

        public HttpContentTypeVariableHandler(HttpServiceProvider httpServiceProvider) {
            _httpServiceProvider = httpServiceProvider;
        }

        public override string Name => "{HTTP_CONTENT_TYPE}";

        public override string Description => "获取http请求内容类型";

        public override string ParseVariable(string template, string source) {
            var res = template;
            var match = Find(res);
            if (match.Success) {
                res = template.Replace(match.Value, _httpServiceProvider.ContentType);
            }

            return res;
        }
    }
}
