using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magicube.Web;
using Magicube.Web.Authencation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magicube.Identity.Web.Identity.Pages.Account.Manage {
    [IdentityDefaultUI(typeof(ExternalLoginsModel<>))]
    public abstract class ExternalLoginsModel : PageModel {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public virtual Task<IActionResult> OnGetAsync() => throw new NotImplementedException();

        public virtual Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey) => throw new NotImplementedException();

        public virtual Task<IActionResult> OnPostLinkLoginAsync(string provider) => throw new NotImplementedException();

        public virtual Task<IActionResult> OnGetLinkLoginCallbackAsync() => throw new NotImplementedException();
    }

    internal class ExternalLoginsModel<TUser> : ExternalLoginsModel where TUser : class, IUser {
        private readonly MagicubeSignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly IUserStore<TUser> _userStore;

        public ExternalLoginsModel(
            UserManager<TUser> userManager,
            MagicubeSignInManager<TUser> signInManager,
            IUserStore<TUser> userStore) {
            _userManager   = userManager;
            _signInManager = signInManager;
            _userStore     = userStore;
        }

        public override async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            string passwordHash = null;
            if (_userStore is IUserPasswordStore<TUser> userPasswordStore) {
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
            }

            ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1;
            return Page();
        }

        public override async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded) {
                StatusMessage = "The external login was not removed.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "The external login was removed.";
            return RedirectToPage();
        }

        public override async Task<IActionResult> OnPostLinkLoginAsync(string provider) {
            await HttpContext.SignOutAsync(AuthencationSchemas.ExternalScheme);

            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        public override async Task<IActionResult> OnGetLinkLoginCallbackAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var info = await _signInManager.GetExternalLoginInfoAsync(userId);
            if (info == null) {
                throw new InvalidOperationException($"Unexpected error occurred loading external login info.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded) {
                StatusMessage = "The external login was not added. External logins can only be associated with one account.";
                return RedirectToPage();
            }

            await HttpContext.SignOutAsync(AuthencationSchemas.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToPage();
        }
    }
}
