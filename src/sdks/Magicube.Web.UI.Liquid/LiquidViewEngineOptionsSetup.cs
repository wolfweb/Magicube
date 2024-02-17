using Fluid;
using Fluid.ViewEngine;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Magicube.Web.UI.Liquid.FileProviders;
using Magicube.Data.Abstractions.ViewModel;
using Fluid.MvcViewEngine;
using Microsoft.AspNetCore.Hosting;

namespace Magicube.Web.UI.Liquid {
    public class LiquidViewEngineOptionsSetup : IConfigureOptions<FluidMvcViewOptions> {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LiquidViewEngineOptionsSetup(IServiceProvider serviceProvider, IWebHostEnvironment webHostEnvironment) {
            _serviceProvider = serviceProvider;
            _webHostEnvironment = webHostEnvironment;
        }
        public void Configure(FluidMvcViewOptions options) {
            if (options.PartialsFileProvider == null) {
                options.PartialsFileProvider = new FileProviderMapper(_webHostEnvironment.ContentRootFileProvider, "Views");
            }

            options.ViewsFileProvider = _serviceProvider.GetService<ILiquidViewProvider>();
            if (options.ViewsFileProvider == null) {
                options.ViewsFileProvider = new FileProviderMapper(_webHostEnvironment.ContentRootFileProvider, "Views");
            }

            options.ViewsLocationFormats.Clear();
            options.ViewsLocationFormats.Add("/{1}/{0}" + Constants.ViewExtension);
            options.ViewsLocationFormats.Add("/Shared/{0}" + Constants.ViewExtension);

            options.PartialsLocationFormats.Clear();
            options.PartialsLocationFormats.Add("{0}" + Constants.ViewExtension);
            options.PartialsLocationFormats.Add("/Partials/{0}" + Constants.ViewExtension);
            options.PartialsLocationFormats.Add("/Partials/{1}/{0}" + Constants.ViewExtension);
            options.PartialsLocationFormats.Add("/Shared/Partials/{0}" + Constants.ViewExtension);

            options.LayoutsLocationFormats.Clear();
            options.LayoutsLocationFormats.Add("/Shared/{0}" + Constants.ViewExtension);

            options.TemplateOptions.MemberAccessStrategy.Register<DynamicEntity, object>((source, name) => source[name]);
            options.TemplateOptions.MemberAccessStrategy.Register<Dictionary<string, object>, object>((source, name) => source[name]);
            options.TemplateOptions.MemberAccessStrategy.Register<IEntityViewModel, object>((source, name) => source[name]);
        }
    }
}
