using System.ComponentModel.DataAnnotations;

namespace Magicube.OpenIdCore.ViewModels.Account {
    public class ExternalLoginConfirmationViewModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
