using Microsoft.AspNetCore.Mvc;

namespace Magicube.Web {
    public static class UrlHelperExtensions {
        public static string GetBaseUrl(this IUrlHelper url) {
            var request = url.ActionContext.HttpContext.Request;
            var scheme = request.Scheme;
            var host = request.Host.ToUriComponent();
            return $"{scheme}://{host}";
        }

        public static string ToAbsoluteUrl(this IUrlHelper url, string virtualPath) {
            var baseUrl = url.GetBaseUrl();
            var path = url.Content(virtualPath);
            return $"{baseUrl}{path}";
        }
    }
}
