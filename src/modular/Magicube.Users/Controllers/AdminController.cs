using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Identity;
using Magicube.Users.Services;
using Magicube.Users.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Magicube.Users.Controllers {
    public class AdminController : Controller {
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AdminController(
            UserManager<User> userManager,
            UserService userService,
            RoleManager<Role> roleManager) {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
        }

        public IActionResult Index([FromQuery] PageSearchModel model) {
            var pageModel = _userService.Page(model);            
            return View(pageModel);
        }

        public async Task<IActionResult> Editor(int? id) {
            UserViewModel model = null;
            if (id.HasValue) {
                model = await _userService.GetViewModelAsync(id.Value);
            }

            return View(model);
        }

        [HttpPost, ActionName("Editor")]
        public async Task<IActionResult> EditorPost([FromForm] UserViewModel model) {
            model.NotNull();

            if (!ModelState.IsValid) {
                return View(model);
            }

            if (model.Id == 0) {
                var user =  model.Build();
                await _userManager.CreateAsync(user, model.Password);
            } else {
                await _userService.AddOrUpdateAsync(model);
            }

            return View(model);
        }
    }
}
