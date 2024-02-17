using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Forms.Services;
using Magicube.Forms.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Magicube.Forms.Controllers {
    public class AdminController : Controller {
        private readonly DbTableService _dbTableService;
        public AdminController(DbTableService dbTableService) {
            _dbTableService = dbTableService;
        }

        public IActionResult Index(PageSearchModel model) {
            var result = _dbTableService.Page(model);
            return View(result);
        }

        public async Task<IActionResult> Editor(int? id) {
            DbTableViewModel model = null;
            if (id.HasValue) {
                model = await _dbTableService.GetViewModelAsync(id.Value);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Editor(DbTableViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            
            model.NotNull();
            var result = await _dbTableService.AddOrUpdateAsync(model);
            return RedirectToAction("Editor", new { id = result.Id });
        }

        public IActionResult Delete(int id) {
            _dbTableService.Remove(id);
            return RedirectToAction("Index");
        }
    }
}
