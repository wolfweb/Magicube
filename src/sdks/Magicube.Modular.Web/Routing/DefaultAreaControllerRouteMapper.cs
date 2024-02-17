using Magicube.Data.Abstractions;
using Magicube.Web.Configuration;
using Magicube.Web.Sites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Reflection;

namespace Magicube.Modular.Web.Routing {
    public class DefaultAreaControllerRouteMapper : IAreaControllerRouteMapper {
        private const string _defaultAreaPattern = "/{area}/{controller}/{action}/{id?}";
        private readonly ISiteManager _siteManager;

        public int Order => 1000;

        public DefaultAreaControllerRouteMapper(ISiteManager siteManager) {
            _siteManager = siteManager;
        }

        public bool TryMapAreaControllerRoute(IEndpointRouteBuilder routes, ControllerActionDescriptor descriptor) {
            var siteSettings = _siteManager.GetSite();
            if (siteSettings?.Status != SiteStatus.Running) return false;

            var adminSetting = siteSettings.As<SiteAdminSetting>() ?? new SiteAdminSetting();

            bool needAuthorize = descriptor.FilterDescriptors.Any(x => x.Filter is AuthorizeFilter) || descriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>() != null ||
                descriptor.MethodInfo.GetCustomAttribute<AuthorizeAttribute>() != null;

            var pattern = needAuthorize ? (adminSetting.AdminUrlPrefix + _defaultAreaPattern) : _defaultAreaPattern;

            routes.MapAreaControllerRoute(
               name    : descriptor.DisplayName,
               areaName: descriptor.RouteValues["area"],
               pattern : pattern.Replace("{action}", descriptor.ActionName),
               defaults: new { controller = descriptor.ControllerName, action = descriptor.ActionName }
            );
            return true;
        }
    }
}
