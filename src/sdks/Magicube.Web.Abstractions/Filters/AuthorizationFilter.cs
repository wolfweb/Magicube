using Magicube.Core;
using Magicube.Web.Authencation;
using Magicube.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.Web.Filters {
    /// <summary>
    /// authorization 用来授权
    /// </summary>
    public class AuthorizationFilter : IAsyncAuthorizationFilter {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationFilter(IAuthorizationService authorizationService) {
            _authorizationService = authorizationService;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context) {
            if (context.ActionDescriptor.EndpointMetadata.Any(x => x is AllowAnonymousAttribute)) return;

            if (context.ActionDescriptor.EndpointMetadata.All(x => !(x is AuthorizeAttribute))) return;

            if (context.ActionDescriptor is CompiledPageActionDescriptor pageActionDescriptor) {
                await AuthorizeRazorPage(pageActionDescriptor, context);
            } else if(context.ActionDescriptor is ControllerActionDescriptor controllerDescriptor ){
                await AuthorizeController(controllerDescriptor, context);
            }
        }

        /// <summary>
        /// RazorPage只能指定PageModel Authorize, 不能作用于Handler
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task AuthorizeRazorPage(CompiledPageActionDescriptor descriptor, AuthorizationFilterContext context) {
            if (descriptor == null) return;

            var authorizeAttribute = descriptor.PageTypeInfo.GetCustomAttribute<AuthorizeAttribute>();

            if (authorizeAttribute == null && !descriptor.DisplayName.StartsWith("Admin")) return;

            if (!context.HttpContext.User.Identity.IsAuthenticated) {
                return;
            }

            var claim = context.HttpContext.User.FindFirst(x => x.Type == "Status");
            if (claim != null && claim.Value == "Disabled") {
                context.Result = new ForbidResult(AuthencationSchemas.HeaderScheme);
                return;
            }

            if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User, new Permission($"{descriptor.DisplayName}"))) {
                context.Result = new ForbidResult(AuthencationSchemas.HeaderScheme);
                return;
            }
        }

        private async Task AuthorizeController(ControllerActionDescriptor descriptor, AuthorizationFilterContext context) {
            if (descriptor == null) return;

            var authorizeAttribute = descriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>();

            if (authorizeAttribute == null) {
                authorizeAttribute = descriptor.MethodInfo.GetCustomAttribute<AuthorizeAttribute>();
            }

            if (authorizeAttribute == null && descriptor.ControllerName != "Admin") return;

            if (!context.HttpContext.User.Identity.IsAuthenticated) {
                return;
            }

            var claim = context.HttpContext.User.FindFirst(x => x.Type == "Status");
            if (claim != null && claim.Value == "Disabled") {
                if(await Forbidden(context)) {
                    return;
                }
            }

            var permissionName = $"{descriptor.ControllerTypeInfo.Namespace}:{descriptor.ControllerName}:{descriptor.ActionName}";

            if (descriptor.RouteValues.TryGetValue("area", out string v)) {
                if (!v.IsNullOrEmpty())
                    permissionName = $"{v}:{permissionName}";
            }

            if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User, new Permission(permissionName))) {
                if (await Forbidden(context))
                    return;
            }
        }

        private Task<bool> Forbidden(AuthorizationFilterContext context) {
            if (context.HttpContext.Request.IsApiRequest()) {
                context.Result = new ObjectResult(new { Code = 403, Message = "forbidden" });
            } else {
                context.Result = new ForbidResult(AuthencationSchemas.HeaderScheme);
            }
            return Task.FromResult(true);
        }

        private Task<bool> Unauthorized(AuthorizationFilterContext context) {
            if (context.HttpContext.Request.IsApiRequest()) {
                context.Result = new ObjectResult(new { Code = 401, Message = "unauthorized" });
            } else {
                context.Result = new UnauthorizedResult();
            }
            return Task.FromResult(true);
        }
    }
}
