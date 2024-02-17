using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Magicube.Core;
using Magicube.Web;
using Magicube.Web.Authencation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Magicube.Identity.Web.Identity.Pages.Account {
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(LoginModel<>))]
    public abstract class LoginModel : PageModel {
        [BindProperty]
        public InputModel                  Input          { get; set; }                                           
                                           
        public string                      ReturnUrl      { get; set; }
                                           
        [TempData]                         
        public string                      ErrorMessage   { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel {
            [Required]
            [EmailAddress]
            public string Email      { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "ÃÜÂë")]
            public string Password   { get; set; }


            [Display(Name = "Remember me?")]
            public bool   RememberMe { get; set; }
        }

        public virtual Task OnGetAsync(string returnUrl = null) => throw new NotImplementedException();

        public virtual Task<IActionResult> OnPostAsync(string returnUrl = null) => throw new NotImplementedException();
    }

    internal class LoginModel<TUser> : LoginModel where TUser : class, IUser {
        private readonly MagicubeSignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(MagicubeSignInManager<TUser> signInManager, UserManager<TUser> userManager, ILogger<LoginModel> logger) {
            _signInManager = signInManager;
            _userManager   = userManager;
            _logger        = logger;
        }

        public override async Task OnGetAsync(string returnUrl = null) {
            if (!ErrorMessage.IsNullOrEmpty()) {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(AuthencationSchemas.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public override async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid) {
                var user = await  _userManager.FindByEmailAsync(Input.Email);

                if (user == null) {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded) {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor) {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut) {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                } else {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            return Page();
        }
    }
}
