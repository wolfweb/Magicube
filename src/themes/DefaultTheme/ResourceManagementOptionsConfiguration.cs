using Magicube.Data.Abstractions;
using Magicube.Resource;
using Magicube.Web.Sites;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DefaultTheme {
    public class ResourceManagementOptionsSetup : IConfigureOptions<ResourceManagementOptions> {
        private readonly IHostEnvironment _env;
        private readonly ISiteManager _siteManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResourceManagementOptionsSetup(
            ISiteManager siteManager, 
            IHostEnvironment env, 
            IHttpContextAccessor httpContextAccessor) {
            _env                 = env;
            _siteManager         = siteManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Configure(ResourceManagementOptions options) {
            options.ResourceManifests.Add(BuildManifest());

            var siteSettings = _siteManager.GetSite();
            var resourceSettings = siteSettings.As<ResourceSettings>() ?? new ResourceSettings();

            switch (resourceSettings.ResourceDebugMode) {
                case ResourceDebugMode.Enabled:
                    options.DebugMode = true;
                    break;

                case ResourceDebugMode.Disabled:
                    options.DebugMode = false;
                    break;

                case ResourceDebugMode.FromConfiguration:
                    options.DebugMode = !_env.IsProduction();
                    break;
            }

            options.UseCdn          = resourceSettings.UseCdn;
            options.CdnBaseUrl      = resourceSettings.CdnBaseUrl;
            options.AppendVersion   = resourceSettings.AppendVersion;
            options.ContentBasePath = _httpContextAccessor.HttpContext.Request.PathBase.Value;
        }

        private ResourceManifest BuildManifest() {
            var manifest = new ResourceManifest();

            #region admin
            manifest.DefineStyle("main")
                .SetUrl("~/defaulttheme/css/style.css")
                .SetVersion("1.0.0");

            manifest.DefineStyle("admin")
                .SetUrl("~/defaulttheme/css/admin.css")
                .SetVersion("1.0.0");

            manifest.DefineScript("admin")
                .SetUrl("~/defaulttheme/js/admin.js")
                .SetVersion("1.0.0");
            #endregion

            #region jquery
            manifest
                .DefineScript("jQuery")
                .SetUrl("~/defaulttheme/lib/jquery/dist/jquery.min.js", "~/defaulttheme/lib/jquery/dist/jquery.js")
                .SetCdn("https://cdn.bootcdn.net/ajax/libs/jquery/3.6.0/jquery.min.js", "https://cdn.bootcdn.net/ajax/libs/jquery/3.6.0/jquery.js")
                .SetVersion("3.6.0");

            manifest
                .DefineScript("jQuery.slim")
                .SetUrl("~/defaulttheme/lib/jquery/dist/jquery.slim.min.js", "~/defaulttheme/lib/jquery/dist/jquery.slim.js")
                .SetCdn("https://cdn.bootcdn.net/ajax/libs/jquery/3.6.0/jquery.slim.min.js", "https://cdn.bootcdn.net/ajax/libs/jquery/3.6.0/jquery.slim.js")
                .SetVersion("3.6.0");

            #endregion

            #region popper
            manifest
                .DefineScript("popper")
                .SetUrl("~/defaulttheme/lib/popper/umd/popper.min.js", "~/defaulttheme/lib/popper/umd/popper.js")
                .SetCdn("https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js", "https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.js")
                .SetVersion("2.11.6");

            #endregion

            #region bootstrap

            manifest
                .DefineScript("bootstrap")
                .SetUrl("~/defaulttheme/lib/bootstrap/dist/js/bootstrap.min.js", "~/defaulttheme/lib/bootstrap/dist/js/bootstrap.js")
                .SetDependencies("jQuery", "popper")
                .SetCdn("https://cdn.bootcdn.net/ajax/libs/twitter-bootstrap/5.2.0/js/bootstrap.min.js", "https://cdn.bootcdn.net/ajax/libs/twitter-bootstrap/5.2.0/js/bootstrap.js")
                .SetVersion("5.2.0");

            manifest
                .DefineScript("bootstrap-bundle")
                .SetDependencies("jQuery")
                .SetUrl("~/defaulttheme/lib/bootstrap/dist/js/bootstrap.bundle.min.js", "~/defaulttheme/lib/bootstrap/dist/js/bootstrap.bundle.js")
                .SetCdn("https://cdn.bootcdn.net/ajax/libs/twitter-bootstrap/5.2.0/js/bootstrap.bundle.min.js", "https://cdn.bootcdn.net/ajax/libs/twitter-bootstrap/5.2.0/js/bootstrap.bundle.js")
                .SetVersion("5.2.0");

            manifest
                .DefineStyle("bootstrap")
                .SetUrl("~/defaulttheme/lib/bootstrap/dist/css/bootstrap.min.css", "~/defaulttheme/lib/bootstrap/dist/css/bootstrap.css")
                .SetCdn("https://cdn.bootcdn.net/ajax/libs/twitter-bootstrap/5.2.0/css/bootstrap.min.css", "https://cdn.bootcdn.net/ajax/libs/twitter-bootstrap/5.2.0/css/bootstrap.css")
                .SetVersion("5.2.0");

            #endregion

            #region font-awesome
            manifest
                .DefineStyle("font-awesome")
                .SetUrl("~/defaulttheme/lib/font-awesome/css/all.min.css", "~/defaulttheme/lib/font-awesome/css/all.css")
                .SetCdn("https://cdn.bootcdn.net/ajax/libs/font-awesome/5.15.4/css/fontawesome.min.css", "https://cdn.bootcdn.net/ajax/libs/font-awesome/5.15.4/css/fontawesome.css")
                .SetVersion("6.1.2");
            #endregion

            #region bootswatch

            var bootswatchBasePath = "/defaulttheme/lib/bootswatch/dist";

            var themes = new[] {
                "cerulean", "cosmo", "cyborg", "darkly", "flatly", "journal", "litera", "lumen", "lux", "materia", "minty", "morph", "pulse", "quartz", "sandstone", "simplex", "sketchy", "slate", "solar", "spacelab", "superhero", "united", "vapor", "yeti", "zephyr"
            };

            foreach(var theme in themes) {
                manifest
                    .DefineStyle(theme)
                    .SetBasePath(bootswatchBasePath)
                    .SetUrl($"~/{theme}/bootstrap.min.css", $"~/{theme}/bootstrap.css")
                    .SetVersion("5.2.2");
            }
            #endregion

            #region bootstrap-tagsinput
            manifest.DefineStyle("tagsinput")
                .SetUrl("~/defaulttheme/lib/bootstrap-tagsinput/bootstrap-tagsinput.css")
                .SetVersion("0.8.0");

            manifest.DefineScript("tagsinput")
                .SetUrl("~/defaulttheme/lib/bootstrap-tagsinput/bootstrap-tagsinput.min.js", "~/defaulttheme/lib/bootstrap-tagsinput/bootstrap-tagsinput.js")
                .SetVersion("0.8.0");
            #endregion

            #region overlayscrollbars
            manifest.DefineStyle("overlayscrollbars")
                .SetUrl("~/defaulttheme/lib/overlayscrollbars/css/OverlayScrollbars.min.css", "~/defaulttheme/lib/overlayscrollbars/css/OverlayScrollbars.css")
                .SetVersion("1.13.3");

            manifest.DefineScript("overlayscrollbars")
                .SetUrl("~/defaulttheme/lib/overlayscrollbars/js/jquery.overlayScrollbars.min.js", "~/defaulttheme/lib/overlayscrollbars/js/jquery.overlayScrollbars.js")
                .SetVersion("1.13.3");
            #endregion

            #region vue
            manifest.DefineScript("vue")
                .SetUrl("~/defaulttheme/lib/vue/vue.global.min.js", "~/defaulttheme/lib/vue/vue.global.js")
                .SetVersion("3.2.37");
            #endregion

            #region element-plus
            manifest.DefineStyle("element")
                .SetUrl("~/defaulttheme/lib/element/dist/index.css")
                .SetVersion("2.2.12");

            manifest.DefineScript("element")
                .SetUrl("~/defaulttheme/lib/element/dist/index.full.min.js", "~/defaulttheme/lib/element/dist/index.full.js")
                .SetVersion("2.2.12");
            #endregion

            #region signalr

            manifest.DefineScript("signalr")
                .SetUrl("~/defaulttheme/lib/signalr/dist/browser/signalr.min.js", "~/defaulttheme/lib/signalr/dist/browser/signalr.js")
                .SetVersion("6.0.10");

            #endregion

            return manifest;
        }
    }
}
