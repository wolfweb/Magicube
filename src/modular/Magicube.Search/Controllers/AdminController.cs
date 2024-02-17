using Magicube.Core;
using Magicube.ElasticSearch;
using Microsoft.AspNetCore.Mvc;

namespace Magicube.Search.Controllers {
    public class AdminController : Controller {
        private readonly IMagicubeConfigProvider<ElasticSearchOptions> _searchConfigService;

        public AdminController(IMagicubeConfigProvider<ElasticSearchOptions> searchConfigService) {
            _searchConfigService = searchConfigService;
        }

        public IActionResult Index() {
            var setting = _searchConfigService.GetSetting();
            return View(setting);
        }
    }
}
