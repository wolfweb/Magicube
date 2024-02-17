using System;
using System.Threading.Tasks;
using Magicube.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Magicube.Identity.Web.Identity.Pages.Account {
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(LogoutModel<>))]
    public abstract class LogoutModel : PageModel {
        public void OnGet() {
        }

        public virtual Task<IActionResult> OnPost(string returnUrl = null) => throw new NotImplementedException();
    }

    internal class LogoutModel<TUser> : LogoutModel where TUser : class, IUser {
        private readonly MagicubeSignInManager<TUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(MagicubeSignInManager<TUser> signInManager, ILogger<LogoutModel> logger) {
            _signInManager = signInManager;
            _logger        = logger;
        }

        public override async Task<IActionResult> OnPost(string returnUrl = null) {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null) {
                return LocalRedirect(returnUrl);
            } else {
                return RedirectToPage();
            }
        }
    }
}
