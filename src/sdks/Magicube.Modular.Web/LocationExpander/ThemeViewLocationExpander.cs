using Magicube.Web.Sites;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Modular.Web.LocationExpander {
    public class ThemeViewLocationExpander : IViewLocationExpander {
        public ThemeViewLocationExpander() {
            
        }
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations) {
            if (!context.Values.ContainsKey("Theme")) {
                return viewLocations;
            }

            string theme = context.Values["Theme"];
            var result = new List<string>();

            var themePagesPath = $"/{theme}/Pages";
            var themeViewsPath = $"/{theme}/Views";

            if (context.AreaName != null) {
                if (context.PageName != null) {
                    result.Add(themePagesPath + "/{2}/{0}" + RazorViewEngine.ViewExtension);
                } else {
                    result.Add(themeViewsPath + "/{2}/{1}/{0}" + RazorViewEngine.ViewExtension);
                }
            }

            if (context.PageName != null) {
                result.Add(themePagesPath + "/Shared/{0}" + RazorViewEngine.ViewExtension);
            }

            if (context.AreaName != null) {
                result.Add(themeViewsPath + "/{2}/Shared/{0}" + RazorViewEngine.ViewExtension);
            }

            result.Add(themeViewsPath + "/Shared/{0}" + RazorViewEngine.ViewExtension);

            return result.Concat(viewLocations);
        }

        public void PopulateValues(ViewLocationExpanderContext context) {
            var siteManager = context.ActionContext.HttpContext.RequestServices.GetRequiredService<ISiteManager>();
            context.Values["Theme"] = siteManager.GetSite()?.Theme ?? "DefaultTheme";
        }
    }
}
