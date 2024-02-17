using Magicube.Core;
using Magicube.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Magicube.Cors.Controllers {
    public class AdminController : Controller {
        private readonly IStringLocalizer L;
        private readonly IMagicubeConfigProvider<CorsPolicySetting> _corsService;

        public AdminController(IStringLocalizer<AdminController> localizer, IMagicubeConfigProvider<CorsPolicySetting> corsService) {
            L = localizer;
            _corsService = corsService;
        }

        public IActionResult Index() {
            CorsPolicySetting policySetting = _corsService.GetSetting();
            return View(policySetting);
        }

        [HttpPost]
        public IActionResult Index([FromForm] CorsPolicySetting settings) {
            settings.NotNull();
            _corsService.SetSetting(settings);
            return View(settings);
        }
    }
}
