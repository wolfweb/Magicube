using System;
using System.Text;
using System.Threading.Tasks;
using Magicube.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Magicube.Identity.Web.Identity.Pages.Account {
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(ConfirmEmailChangeModel<>))]
    public abstract class ConfirmEmailChangeModel : PageModel {
        [TempData]
        public string StatusMessage { get; set; }

        public virtual Task<IActionResult> OnGetAsync(string userId, string email, string code) => throw new NotImplementedException();
    }

    internal class ConfirmEmailChangeModel<TUser> : ConfirmEmailChangeModel where TUser : class, IUser {
        private readonly UserManager<TUser> _userManager;
        private readonly MagicubeSignInManager<TUser> _signInManager;

        public ConfirmEmailChangeModel(UserManager<TUser> userManager, MagicubeSignInManager<TUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public override async Task<IActionResult> OnGetAsync(string userId, string email, string code) {
            if (userId == null || email == null || code == null) {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded) {
                StatusMessage = "Error changing email.";
                return Page();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded) {
                StatusMessage = "Error changing user name.";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Thank you for confirming your email change.";
            return Page();
        }
    }
}
