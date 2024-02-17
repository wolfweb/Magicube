using Magicube.Core;
using Magicube.Web;
using System.Threading.Tasks;

namespace Magicube.Modular.Web.Complier {
    public class ThemeViewStart : MagicubeRazorPage<dynamic> {
        public override Task ExecuteAsync() {
            if (Site != null && Layout.IsNullOrEmpty()) {
                var layout = ViewData["Layout"];
                if (layout == null) {
                    Layout = "_Layout";
                } else {
                    SetLayout(layout.ToString());
                }
            }
            return Task.CompletedTask;
        }
    }
}
