using Magicube.Data.Abstractions;
using Magicube.Web.UI.Entities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using Microsoft.Extensions.DependencyInjection;
using Magicube.Core.Signals;
using Microsoft.AspNetCore.Hosting;
using Fluid.ViewEngine;
using System.Collections.Generic;
using Fluid.MvcViewEngine;
using Magicube.Web.UI.Liquid.MvcViewEngine;

namespace Magicube.Web.UI.Liquid.FileProviders {
    public interface ILiquidViewProvider : IFileProvider { }
    public class LiquidViewProvider : ILiquidViewProvider {
        private readonly Dictionary<string, LiquidViewInfo> _content = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IFileProvider _partialsFileProvider;

        public LiquidViewProvider(IServiceScopeFactory serviceScopeFactory, IWebHostEnvironment webHostEnvironment) {
            _serviceScopeFactory = serviceScopeFactory;
            _partialsFileProvider = new FileProviderMapper(webHostEnvironment.ContentRootFileProvider, "Views");
        }

        public IDirectoryContents GetDirectoryContents(string subpath) => throw new NotImplementedException();

        public IFileInfo GetFileInfo(string subpath) {
            using (var scoped = _serviceScopeFactory.CreateScope()) {
                var fluidRender = scoped.ServiceProvider.GetService<FluidRendering>() as MagicubeFluidRendering;
                var rep         = scoped.ServiceProvider.GetService<IRepository<WebPage, int>>();
                var signal      = scoped.ServiceProvider.GetService<ISignal>();
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
