using System.ComponentModel.DataAnnotations;

namespace Magicube.OpenIdCore.ViewModels.Account {
    public class ForgotPasswordViewModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
