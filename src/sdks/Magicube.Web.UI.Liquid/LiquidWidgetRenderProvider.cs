using Fluid;
using Fluid.ViewEngine;
using Magicube.Web.UI;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Magicube.Web.UI.Liquid {
    public class LiquidWidgetRenderProvider : IWidgetRenderProvider {

        private readonly ConcurrentDictionary<string, IFluidTemplate> _widgets;
        private readonly FluidViewEngineOptions _fluidViewEngine;

        public LiquidWidgetRenderProvider(IOptions<FluidViewEngineOptions> options) {
            _fluidViewEngine = options.Value;
            _widgets         = new ConcurrentDictionary<string, IFluidTemplate>();
        }

        public async Task<IHtmlContent> RenderAsync(IWidget widget) {
            var template = _widgets.GetOrAdd(widget.Name, key => {
                if (!_fluidViewEngine.Parser.TryParse(widget.Content, out var v, out var errors)) {
                    throw new WebUIException($"parse widget : {widget} error\n {errors}");
                }
                return v;
            });

            var content = await template.RenderAsync();
            return new HtmlString(content);
        }
    }
}
