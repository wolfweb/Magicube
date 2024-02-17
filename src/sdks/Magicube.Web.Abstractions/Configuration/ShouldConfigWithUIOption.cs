using Magicube.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Web.Configuration {
    public abstract class ShouldConfigWithUIOption : ShouldConfigOption {
        private IUrlHelper _urlHelper;
        protected readonly IHttpContextAccessor HttpContextAccessor;
        public ShouldConfigWithUIOption(IHttpContextAccessor httpContextAccessor) {
            HttpContextAccessor = httpContextAccessor;
        }

        public void DoRedirect() => HttpContextAccessor.HttpContext.Response.Redirect(ConfigPath(), true);

        private string ConfigPath() {
            if (_urlHelper == null) {
                var actionContext = GetService<IActionContextAccessor>().ActionContext;
                var factory       = GetService<IUrlHelperFactory>();
                _urlHelper        = factory.GetUrlHelper(actionContext);
            }

            return _urlHelper.RouteUrl(new UrlRouteContext { Values = Routes });
        }

        public abstract RouteValueDictionary Routes { get; }

        protected T GetService<T>() {
            return HttpContextAccessor.HttpContext.RequestServices.GetRequiredService<T>();
        }
    }
}
