using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Magicube.Web.Middlewares {
    public class PoweredByMiddleware {
        private readonly RequestDelegate _next;

        public PoweredByMiddleware(RequestDelegate next) {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext) {
            httpContext.Response.Headers["X-Powered-By"] = "Magicube";
            return _next.Invoke(httpContext);
        }
    }
}
