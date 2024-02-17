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
    [IdentityDefaultUI(typeof(LoginWithRecoveryCodeModel<>))]
    public abstract class LoginWithRecoveryCodeModel : PageModel {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel {
            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
        }

        public virtual Task<IActionResult> OnGetAsync(string returnUrl = null) => throw new NotImplementedException();

        public virtual Task<IActionResult> OnPostAsync(string returnUrl = null) => throw new NotImplementedException();
    }

    internal class LoginWithRecoveryCodeModel<TUser> : LoginWithRecoveryCodeModel where TUser : class, IUser {
        private readonly MagicubeSignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<LoginWithRecoveryCodeModel> _logger;

        public LoginWithRecoveryCodeModel(
            MagicubeSignInManager<TUser> signInManager,
            UserManager<TUser> userManager,
            ILogger<LoginWithRecoveryCodeModel> logger) {
            _signInManager = signInManager;
            _userManager   = userManager;
            _logger        = logger;
        }

        public override async Task<IActionResult> OnGetAsync(string returnUrl = null) {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            if (!ModelState.IsValid) {
                return Page();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            var userId = await _userManager.GetUserIdAsync(user);

            if (result.Succeeded) {
                _logger.LogInformation("User logged in with a recovery code.");
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            if (result.IsLockedOut) {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            } else {
                _logger.LogWarning("Invalid recovery code entered.");
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return Page();
            }
        }
    }
}
