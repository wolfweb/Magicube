using Magicube.ElasticSearch7;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magicube.Search.Controllers {
    [Authorize]
    public class SearchManageController : Controller {
        private readonly IElastic7SearchService _elastic7SearchService;
        public SearchManageController(IElastic7SearchService elastic7SearchService) {
            _elastic7SearchService = elastic7SearchService;
        }

        public IActionResult Index() { 
            return View();
        }
    }
}
