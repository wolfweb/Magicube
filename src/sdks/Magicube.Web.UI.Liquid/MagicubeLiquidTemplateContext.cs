using Fluid;
using System;

namespace Magicube.Web.UI.Liquid {
    public class MagicubeLiquidTemplateContext : TemplateContext {
        public MagicubeLiquidTemplateContext(IServiceProvider services) {
            Services = services;
        }

        public MagicubeLiquidTemplateContext(object model, IServiceProvider services) : base(model) {
            Services = services;
        }

        public IServiceProvider Services { get; }
    }
}
