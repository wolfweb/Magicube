using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Http {
    public class RedirectExecutionResult : ActivityExecutionResult {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RedirectExecutionResult(string url, bool permanent, IHttpContextAccessor httpContextAccessor) {
            Url       = url;
            Permanent = permanent;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Url { get; }
        public bool   Permanent { get; }

        public override ValueTask ExecuteAsync(ExecuteflowContext executionContext, CancellationToken cancellationToken = default) {
            var httpContext = _httpContextAccessor.HttpContext;
            var response = httpContext.Response;
            response.Redirect(Url, Permanent);
            return base.ExecuteAsync(executionContext, cancellationToken);
        }
    }
}
