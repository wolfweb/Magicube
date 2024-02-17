using Fluid.MvcViewEngine;
using Magicube.Core;
using Magicube.Core.Modular;
using Magicube.Web.UI.Liquid;
using Magicube.Web.UI.Liquid.FileProviders;
using Magicube.Web.UI.Liquid.MvcViewEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddLiquid(this WebServiceBuilder builder) {
            if (builder.MvcBuilder == null) {
                builder.AddMvc();
            }

            builder.Services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.Add(typeof(MagicubeLiquidTemplateContext).Assembly);
            });

            builder.Services.Configure<FluidMvcViewOptions>(options => {
                options.Parser = new MagicubeLiquidParser();
            });

            builder.Services.AddSingleton<ILiquidViewProvider, LiquidViewProvider>();
            builder.Services.AddTransient<IConfigureOptions<MvcViewOptions>, MvcViewOptionsSetup>();
            builder.Services.AddSingleton(typeof(FluidRendering), typeof(MagicubeFluidRendering));
            builder.Services.AddSingleton(typeof(IFluidViewEngine), typeof(MagicubeFluidViewEngine));
            builder.Services.ConfigureOptions<LiquidViewEngineOptionsSetup>();

            return builder;
        }

        class MvcViewOptionsSetup : IConfigureOptions<MvcViewOptions> {
            private readonly IFluidViewEngine _fluidViewEngine;
            public MvcViewOptionsSetup(IFluidViewEngine fluidViewEngine) {
                _fluidViewEngine = fluidViewEngine;
            }
            
            public void Configure(MvcViewOptions options) {
                options.ViewEngines.Add(_fluidViewEngine);
            }
        }
    }
}
