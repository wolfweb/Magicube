using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Magicube.Identity.Web.Identity.Pages.Account.Manage {
    [IdentityDefaultUI(typeof(PersonalDataModel<>))]
    public abstract class PersonalDataModel : PageModel {
        public virtual Task<IActionResult> OnGet() => throw new NotImplementedException();
    }

    internal class PersonalDataModel<TUser> : PersonalDataModel where TUser : class {
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public PersonalDataModel(
            UserManager<TUser> userManager,
            ILogger<PersonalDataModel> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        public override async Task<IActionResult> OnGet() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
