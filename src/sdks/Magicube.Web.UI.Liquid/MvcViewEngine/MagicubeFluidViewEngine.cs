using Fluid.MvcViewEngine;
using Fluid.ViewEngine;
using Magicube.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Magicube.Web.UI.Liquid.MvcViewEngine {
    public class MagicubeFluidViewEngine : IFluidViewEngine {
        private MagicubeFluidRendering _fluidRendering;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private const string ControllerKey = "controller";
        private const string AreaKey = "area";
        private FluidMvcViewOptions _options;

        public MagicubeFluidViewEngine(
            FluidRendering fluidRendering,
            IOptions<FluidMvcViewOptions> optionsAccessor,
            IWebHostEnvironment hostingEnvironment
            ) {
            _options            = optionsAccessor.Value;
            _fluidRendering     = fluidRendering as MagicubeFluidRendering;
            _hostingEnvironment = hostingEnvironment;

            _fluidRendering.NotNull();
        }

        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage) {
            return LocatePageFromViewLocations(context, viewName);
        }

        private ViewEngineResult LocatePageFromViewLocations(ActionContext actionContext, string viewName) {
            var controllerName = GetNormalizedRouteValue(actionContext, ControllerKey);
            var areaName = GetNormalizedRouteValue(actionContext, AreaKey);

            var fileProvider = _options.ViewsFileProvider ?? _hostingEnvironment.ContentRootFileProvider;

            var checkedLocations = new List<string>();

            foreach (var location in _options.ViewsLocationFormats) {
                var view = String.Format(location, viewName, controllerName, areaName);

                if (fileProvider.GetFileInfo(view).Exists) {
                    return ViewEngineResult.Found(viewName, new MagicubeFluidView(view, _fluidRendering));
                }

                checkedLocations.Add(view);
            }

            return ViewEngineResult.NotFound(viewName, checkedLocations);
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage) {
            var applicationRelativePath = GetAbsolutePath(executingFilePath, viewPath);

            if (!(IsApplicationRelativePath(viewPath) || IsRelativePath(viewPath))) {
                return ViewEngineResult.NotFound(applicationRelativePath, Enumerable.Empty<string>());
            }

            return ViewEngineResult.Found("Default", new MagicubeFluidView(applicationRelativePath, _fluidRendering));
        }

        public string GetAbsolutePath(string executingFilePath, string pagePath) {
            if (pagePath.IsNullOrEmpty()) {
                return pagePath;
            }

            if (IsApplicationRelativePath(pagePath)) {
                return pagePath.Replace("~/", "");
            }

            if (!IsRelativePath(pagePath)) {
                return pagePath;
            }

            if (executingFilePath.IsNullOrEmpty()) {
                return "/" + pagePath;
            }

            var index = executingFilePath.LastIndexOf('/');
            Debug.Assert(index >= 0);
            return executingFilePath.Substring(0, index + 1) + pagePath;
        }


        private static bool IsApplicationRelativePath(string name) {
            Debug.Assert(!name.IsNullOrEmpty());
            return name[0] == '~' || name[0] == '/';
        }

        private static bool IsRelativePath(string name) {
            Debug.Assert(!name.IsNullOrEmpty());
            return name.EndsWith(Constants.ViewExtension, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetNormalizedRouteValue(ActionContext context, string key) {
            context.NotNull(nameof(context));
            key.NotNull(nameof(key));

            if (!context.RouteData.Values.TryGetValue(key, out object routeValue)) {
                return null;
            }

            var actionDescriptor = context.ActionDescriptor;
            string normalizedValue = null;

            if (actionDescriptor.RouteValues.TryGetValue(key, out string value) && !value.IsNullOrEmpty()) {
                normalizedValue = value;
            }

            var stringRouteValue = routeValue?.ToString();
            if (string.Equals(normalizedValue, stringRouteValue, StringComparison.OrdinalIgnoreCase)) {
                return normalizedValue;
            }

            return stringRouteValue;
        }
    }
}
