using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magicube.Email.Controllers {
    [Authorize]
    public class MailManageController : Controller {
        public MailManageController() {
            
        }
    }
}
