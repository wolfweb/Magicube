using Fluid;
using Fluid.MvcViewEngine;
using Fluid.ViewEngine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.Web.UI.Liquid.MvcViewEngine {
    public class MagicubeFluidRendering : FluidRendering {
        private readonly IDictionary _innerCache;
        private readonly FluidViewRenderer _fluidViewRenderer;
        public MagicubeFluidRendering(IOptions<FluidMvcViewOptions> optionsAccessor, IWebHostEnvironment hostingEnvironment) 
            : base(optionsAccessor, hostingEnvironment) {
            _fluidViewRenderer                = new FluidViewRenderer(optionsAccessor.Value);
            _innerCache = (IDictionary)_fluidViewRenderer.GetType().GetField("_cache", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance).GetValue(_fluidViewRenderer);
        }

        public async Task RenderAsync(TextWriter writer, string path, object model, ViewDataDictionary viewData, ModelStateDictionary modelState,IServiceProvider serviceProvider) {
            TemplateContext context = model == null ? new MagicubeLiquidTemplateContext(serviceProvider) : new MagicubeLiquidTemplateContext(model, serviceProvider);
            context.SetValue("ViewData", viewData);
            context.SetValue("ModelState", modelState);
            context.SetValue("Model", model);

            await _fluidViewRenderer.RenderViewAsync(writer, path, context);
        }

        public void ExpireView(string subPath, IFileProvider fileProvider) {
            if (_innerCache.Contains(fileProvider)) {
                var cache = _innerCache[fileProvider];
                //todo: 此处可优化
                var templateCache = (IDictionary<string, IFluidTemplate>)cache.GetType().GetField("TemplateCache").GetValue(cache);
                templateCache.Remove(subPath);
            }
        }
    }
}
