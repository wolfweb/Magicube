using Magicube.Core;
using Magicube.Core.Signals;
using Magicube.Data.Abstractions;
using Magicube.Web.UI.Entities;
using Magicube.Web.UI.Liquid.MvcViewEngine;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Fluid.ViewEngine;
using System.Collections.Generic;
using Fluid.MvcViewEngine;

namespace Magicube.Web.UI.Liquid.FileProviders {
    public interface ILiquidViewProvider : IFileProvider { }
    public class LiquidViewProvider : ILiquidViewProvider {
        private readonly Dictionary<string, LiquidViewInfo> _content = new();
        
        private readonly Application _app;
        private readonly IFileProvider _partialsFileProvider;

        public LiquidViewProvider(Application app, IWebHostEnvironment webHostEnvironment) {
            _app = app;
            _partialsFileProvider = new FileProviderMapper(webHostEnvironment.ContentRootFileProvider, "Views");
        }

        public IDirectoryContents GetDirectoryContents(string subpath) => throw new NotImplementedException();

        public IFileInfo GetFileInfo(string subpath) {
            using (var scoped = _app.CreateScope()) {
                var fluidRender = scoped.GetService<FluidRendering>() as MagicubeFluidRendering;
                var rep         = scoped.GetService<IRepository<WebPage, int>>();
                var signal      = scoped.GetService<ISignal>();
                var result      = new LiquidViewInfo(subpath, signal, rep);
                if (result.Exists) {
                    result.ChangeToken.RegisterChangeCallback(obj => {
                        fluidRender.ExpireView(subpath, this);
                    }, null);
                    return result;
                }

                return _partialsFileProvider.GetFileInfo(subpath);
            }
        }

        public IChangeToken Watch(string filter) {
            if (_content.TryGetValue(filter, out var fileInfo)) {
                return fileInfo.ChangeToken;
            }

            return NullChangeToken.Singleton;
        }
    }
}
