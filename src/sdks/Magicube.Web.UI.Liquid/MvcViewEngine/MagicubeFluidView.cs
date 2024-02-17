using Fluid.MvcViewEngine;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Threading.Tasks;

namespace Magicube.Web.UI.Liquid.MvcViewEngine {
    public class MagicubeFluidView : IView {
        private string _path;
        private MagicubeFluidRendering _fluidRendering;

        public MagicubeFluidView(string path, MagicubeFluidRendering fluidRendering) {
            _path = path;
            _fluidRendering = fluidRendering;
        }

        public string Path {
            get {
                return _path;
            }
        }

        public async Task RenderAsync(ViewContext context) {
            await _fluidRendering.RenderAsync(
                context.Writer,
                Path,
                context.ViewData.Model,
                context.ViewData,
                context.ModelState,
                context.HttpContext.RequestServices
                );
        }
    }
}
