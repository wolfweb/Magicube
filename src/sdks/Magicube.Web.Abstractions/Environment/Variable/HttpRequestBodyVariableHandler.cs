using Magicube.Core.Environment.Variable;

namespace Magicube.Web.Environment.Variable {
    public class HttpRequestBodyVariableHandler : BaseVariableHandler {
        private readonly HttpServiceProvider _httpServiceProvider;

        public HttpRequestBodyVariableHandler(HttpServiceProvider httpServiceProvider) {
            _httpServiceProvider = httpServiceProvider;
        }

        public override string Name => "{HTTP_REQUEST_BODY}";

        public override string Description => "获取http请求内容";

        public override string ParseVariable(string template, string source) {
            var res = template;
            var match = Find(res);
            if (match.Success) {
                res = template.Replace(match.Value, _httpServiceProvider.Body);
            }

            return res;
        }
    }
}
