using Magicube.Core.Modular;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using System;

namespace Magicube.Modular.Web {
    public class ModularApplicationModelProvider : IApplicationModelProvider {
        private readonly IModularManager _modularManager;

        public ModularApplicationModelProvider(IModularManager modularManager) {
            _modularManager = modularManager;
        }

        public int Order => 1000;

        public void OnProvidersExecuted(ApplicationModelProviderContext context) {
            foreach (var controller in context.Result.Controllers) {
                var controllerType = controller.ControllerType.AsType();
                var modular = _modularManager.FindModular(controllerType);
                if (modular != null) {
                    controller.RouteValues.Add("area", modular.Descriptor.Name);
                } 
            }
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            foreach (var controller in context.Result.Controllers) {
                if (controller.ControllerType.Name.StartsWith("Admin")) {
                    controller.Filters.Add(new AuthorizeFilter());
                }
            }
        }
    }    
}
