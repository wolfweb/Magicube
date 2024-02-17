using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Magicube.Localization {
    public class LanguageRouteConstraint : IRouteConstraint {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection) {
            if (!values.ContainsKey("lang")) {
                return false;
            }
            var lang = values["lang"].ToString().ToLower();
            var options = httpContext.RequestServices.GetService<RequestLocalizationOptions>();
            return options.SupportedCultures.Where(x => x.Name.ToLower() == lang).Count() > 0;
        }
    }
}
