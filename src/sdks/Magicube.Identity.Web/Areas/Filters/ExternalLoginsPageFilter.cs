using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Identity.Web.Filters {
    internal class ExternalLoginsPageFilter : IAsyncPageFilter {
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next) {
            var result = await next();
            if (result.Result is PageResult page) {
                var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
                var hasExternalLogins = schemes.Any();

                page.ViewData["ManageNav.HasExternalLogins"] = hasExternalLogins;
            }
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) {
            return Task.CompletedTask;
        }
    }
}
