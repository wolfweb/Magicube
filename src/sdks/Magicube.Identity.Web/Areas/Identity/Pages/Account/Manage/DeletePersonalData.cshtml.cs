using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Magicube.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Magicube.Identity.Web.Identity.Pages.Account.Manage {
    [IdentityDefaultUI(typeof(DeletePersonalDataModel<>))]
    public abstract class DeletePersonalDataModel : PageModel {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public virtual Task<IActionResult> OnGet() => throw new NotImplementedException();

        public virtual Task<IActionResult> OnPostAsync() => throw new NotImplementedException();
    }

    internal class DeletePersonalDataModel<TUser> : DeletePersonalDataModel where TUser : class, IUser {
        private readonly UserManager<TUser> _userManager;
        private readonly MagicubeSignInManager<TUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        public DeletePersonalDataModel(
            UserManager<TUser> userManager,
            MagicubeSignInManager<TUser> signInManager,
            ILogger<DeletePersonalDataModel> logger) {
            _userManager   = userManager;
            _signInManager = signInManager;
            _logger        = logger;
        }

        public override async Task<IActionResult> OnGet() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public override async Task<IActionResult> OnPostAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword) {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password)) {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded) {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User deleted themselves.");

            return Redirect("~/");
        }
    }
}
