using Magicube.Core;
using Magicube.Net.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Magicube.Email.Controllers {
    public class AdminController : Controller {
        private readonly IStringLocalizer L;
        private readonly IMagicubeConfigProvider<MailOption> _emailSettingService;

        public AdminController(
            IStringLocalizer<AdminController> localizer, 
            IMagicubeConfigProvider<MailOption> emailSettingService
            ) {
            L                    = localizer;
            _emailSettingService = emailSettingService;
        }

        public IActionResult Index() {
            var settings = _emailSettingService.GetSetting();
            return View(settings);
        }

        [HttpPost]
        public IActionResult Index([FromForm] MailOption settings) {
            settings.NotNull();

            if (!ModelState.IsValid) {
                return View(settings);
            }

            _emailSettingService.SetSetting(settings);
            return View(settings);
        }
    }
}
