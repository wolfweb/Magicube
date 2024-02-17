using Fluid.MvcViewEngine;
using Fluid.ViewEngine;
using Magicube.Web.UI.Liquid;
using Magicube.Web.UI.Liquid.FileProviders;
using Magicube.Web.UI.Liquid.MvcViewEngine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace Magicube.Web.UI.Test {
    public class LiquidProviderTest {
        private readonly IServiceProvider ServiceProvider;

        public LiquidProviderTest() {
            var hostEnv = new Mock<IWebHostEnvironment>();

            ServiceProvider = new ServiceCollection()
                .AddSingleton(hostEnv.Object)
                .AddHttpContextAccessor()
                .AddWeb(builder => {
                    builder.AddMvc()
                    .AddLiquid();
                }).BuildServiceProvider();
        }

        [Fact]
        public void Func_Liquid_Test() {
            var viewEngine = ServiceProvider.GetService<IFluidViewEngine>();
            Assert.NotNull(viewEngine);

            var render = ServiceProvider.GetService<FluidRendering>();
            Assert.NotNull(render);
            Assert.Equal(typeof(MagicubeFluidRendering), render.GetType());

            var options = ServiceProvider.GetService<IOptions<FluidViewEngineOptions>>();
            Assert.NotNull(options);
            Assert.NotNull(options.Value);

            var viewEngineOption = options.Value;
            Assert.Equal(typeof(LiquidViewProvider), viewEngineOption.ViewsFileProvider.GetType());
        }
    }
}
