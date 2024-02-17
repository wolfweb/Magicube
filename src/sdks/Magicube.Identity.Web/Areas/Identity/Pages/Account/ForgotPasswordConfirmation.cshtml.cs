
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magicube.Identity.Web.Identity.Pages.Account {
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel {
        public void OnGet() {
        }
    }
}
