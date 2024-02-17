using Magicube.Cache.Abstractions;
using Magicube.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Magicube.Caching.Controllers {
    public class AdminController : Controller {
        private readonly IStringLocalizer L;
        private readonly CacheOptions _cacheOptions;
        private readonly IMagicubeConfigProvider<CacheSetting> _cacheService;

        public AdminController(
            IOptions<CacheOptions> options,
            IStringLocalizer<AdminController> localizer, 
            IMagicubeConfigProvider<CacheSetting> cacheService
            ) {
            L             = localizer;
            _cacheOptions = options.Value;
            _cacheService = cacheService;
        }

        public IActionResult Index() {
            CacheSetting setting = _cacheService.GetSetting();
            ViewBag.CacheOptions = _cacheOptions;
            return View(setting);
        }

        [HttpPost]
        public IActionResult Index(CacheSetting setting) {
            setting.NotNull();

            if (!ModelState.IsValid) {
                return View(setting);
            }

            _cacheService.SetSetting(setting);
            return RedirectToAction("Index");
        }
    }
}
