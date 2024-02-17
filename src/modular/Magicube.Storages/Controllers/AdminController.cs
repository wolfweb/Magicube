using Magicube.Data.Abstractions;
using Magicube.Storage.Abstractions.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Magicube.Storages.Controllers {
    public class AdminController : Controller {
        private readonly IStorageStoreManage _cloudStorageService;

        public AdminController(IStorageStoreManage cloudStorageService) {
            _cloudStorageService = cloudStorageService;
        }

        public IActionResult Index(PageSearchModel model) {
            var data  = _cloudStorageService.Paging(model);
            return View(data);
        }
    }
}
