using Magicube.Web.Sites;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authorization;

namespace Magicube.Web {
    public class MagicubeRazorPage<TModel> : RazorPage<TModel> {
        private IViewLocalizer _localizer;
        public MagicubeRazorPage() {
            
        }

        public IViewLocalizer L {
            get {
                if (_localizer == null) {
                    _localizer = Context.RequestServices.GetRequiredService<IViewLocalizer>();
                    ((IViewContextAware)_localizer).Contextualize(ViewContext);
                }
                return _localizer;
            }
        }

        public override Task ExecuteAsync() {
            return Task.CompletedTask;
        }

        public IAuthorizationService AuthorizationService => Context.RequestServices.GetService<IAuthorizationService>();

        public DefaultSite Site {
            get {
                return Context.RequestServices.GetService<ISiteManager>().GetSite();
            }
        }

        /// <summary>
        /// 设置使用的布局，~开头可以设置模块内自定义的布局
        /// </summary>
        /// <param name="layout"></param>
        protected void SetLayout(string layout) {
            if (layout.StartsWith("~")) { 
                Layout = $"~/{Context.Request.RouteValues["area"]}{layout.TrimStart('~')}";
            } else {
                Layout = $"~/{Site.Theme ?? "DefaultTheme"}/Views/Shared/{layout}.cshtml";
            }
        }

        public string Author  => "wolfweb";
        public string Website => "http://www.magicube.com";
    }
}
