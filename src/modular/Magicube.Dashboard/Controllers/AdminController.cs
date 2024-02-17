using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Magicube.Dashboard.Controllers {
    public class AdminController : Controller {
        private readonly IStringLocalizer L;

        public AdminController(IStringLocalizer<AdminController> localizer) {
            L = localizer;
        }

        public IActionResult Index() {
            return View();
        }
    }
}