using System;
using System.Text;
using System.Threading.Tasks;
using Magicube.Net.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Magicube.Identity.Web.Identity.Pages.Account {
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(RegisterConfirmationModel<>))]
    public class RegisterConfirmationModel : PageModel {
        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public virtual Task<IActionResult> OnGetAsync(string email, string returnUrl = null) => throw new NotImplementedException();
    }

    internal class RegisterConfirmationModel<TUser> : RegisterConfirmationModel where TUser : class {
        private readonly UserManager<TUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<TUser> userManager, IEmailSender sender) {
            _userManager = userManager;
            _sender = sender;
        }

        public override async Task<IActionResult> OnGetAsync(string email, string returnUrl = null) {
            if (email == null) {
                return RedirectToPage("/Index");
            }
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            DisplayConfirmAccountLink = _sender is EmailSender;
            if (DisplayConfirmAccountLink) {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Identity/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

            return Page();
        }
    }
}
