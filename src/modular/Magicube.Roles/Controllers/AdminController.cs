using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Roles.Services;
using Magicube.Roles.ViewModels;
using Magicube.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Roles.Controllers {
    public class AdminController : Controller{
        private readonly ILogger _logger;
        private readonly IStringLocalizer L;
        private readonly RoleService _roleService;
        private readonly IPermissionProvider _permissionProvider;

        public AdminController(
            ILogger<AdminController> logger,
            RoleService roleService,
            IStringLocalizer<AdminController> localizer,
            IPermissionProvider permissionProvider) {
            L                     = localizer;
            _logger               = logger;
            _roleService          = roleService;
            _permissionProvider  = permissionProvider;
        }

        public IActionResult Index([FromQuery] PageSearchModel model) {
            var pageModel = _roleService.Page(model);
            return View(pageModel);
        }

        public async Task<IActionResult> Editor(int? id) {
            RoleViewModel model = null;
            if (id.HasValue) {
                model = await _roleService.GetViewModelAsync(id.Value);
            }

            ViewBag.Permissions = _permissionProvider.GetPermissions().ToArray();

            return View(model);
        }

        [HttpPost, ActionName("Editor")]
        public async Task<IActionResult> EditorPost([FromForm] RoleViewModel model) {
            model.NotNull();

            await _roleService.AddOrUpdateAsync(model);
            return View(model);
        }
    }
}
