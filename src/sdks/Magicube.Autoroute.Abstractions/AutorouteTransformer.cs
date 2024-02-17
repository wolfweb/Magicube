using System.Threading.Tasks;
using Magicube.Web.Sites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Magicube.Autoroute.Abstractions {
    public class AutorouteTransformer : DynamicRouteValueTransformer {
        private readonly AutorouteOption _options;
        private readonly ISiteManager _siteManager;
        private readonly IAutorouteEntriesProvider _autorouteEntriesProvider;
        public AutorouteTransformer(
            IOptions<AutorouteOption> options,
            ISiteManager siteManager,
            IAutorouteEntriesProvider autorouteEntriesProvider
            ) {
            _options                  = options.Value;
            _siteManager              = siteManager;
            _autorouteEntriesProvider = autorouteEntriesProvider;
        }
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values) {
            var site = _siteManager.GetSite();
            if (site.Status != SiteStatus.Running) return null;
            var requestPath = httpContext.Request.Path.Value;

            var (found, entry) = await _autorouteEntriesProvider.TryGetAutorouteEntryAsync(requestPath);

            if (found) {
                var routeValues = new RouteValueDictionary(_options.AutorouteValues) {
                    [_options.ContentIdKey] = entry.ContentId
                };

                values.Clear();

                return routeValues;
            }

            return null;
        }
    }
}
