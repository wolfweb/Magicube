using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Magicube.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Magicube.Identity.Web.Identity.Pages.Account {
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(LoginWith2faModel<>))]
    public abstract class LoginWith2faModel : PageModel {
        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public virtual Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null) => throw new NotImplementedException();

        public virtual Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null) => throw new NotImplementedException();
    }

    internal class LoginWith2faModel<TUser> : LoginWith2faModel where TUser : class, IUser {
        private readonly MagicubeSignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<LoginWith2faModel> _logger;

        public LoginWith2faModel(
            MagicubeSignInManager<TUser> signInManager,
            UserManager<TUser> userManager,
            ILogger<LoginWith2faModel> logger) {
            _signInManager = signInManager;
            _userManager   = userManager;
            _logger        = logger;
        }

        public override async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null) {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null) {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null) {
            if (!ModelState.IsValid) {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            var userId = await _userManager.GetUserIdAsync(user);

            if (result.Succeeded) {
                _logger.LogInformation("User logged in with 2fa.");
                return LocalRedirect(returnUrl);
            } else if (result.IsLockedOut) {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            } else {
                _logger.LogWarning("Invalid authenticator code entered.");
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return Page();
            }
        }
    }
}
